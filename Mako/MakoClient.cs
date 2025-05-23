// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mako.Engine;
using Mako.Global.Enum;
using Mako.Global.Exception;
using Mako.Model;
using Mako.Net;
using Mako.Net.EndPoints;
using Mako.Preference;
using Mako.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Misaki;

namespace Mako;

public partial class MakoClient : ICancellable, IDisposable, IAsyncDisposable, IDownloadHttpClientService, IGetArtworkService, IPostFavoriteService
{
    /// <summary>
    /// Create a new <see cref="MakoClient" /> based on given <see cref="Configuration" />, <see cref="TokenResponse" />
    /// </summary>
    /// <param name="configuration">The <see cref="Configuration" /></param>
    /// <param name="logger"></param>
    public MakoClient(MakoClientConfiguration configuration, ILogger logger)
    {
        Logger = logger;
        Configuration = configuration;
    }

    public void Build(TokenResponse tokenResponse)
    {
        IsBuilt = true;
        Provider = BuildServiceProvider(Services, tokenResponse);
    }

    public void Build(string refreshToken) => Build(TokenResponse.CreateFromRefreshToken(refreshToken));

    /// <summary>
    /// Injects necessary dependencies
    /// </summary>
    /// <returns>The <see cref="ServiceProvider" /> contains all the required dependencies</returns>
    private ServiceProvider BuildServiceProvider(IServiceCollection serviceCollection, TokenResponse firstTokenResponse)
    {
        _ = serviceCollection
            .AddSingleton(this)
            .AddSingleton<PixivApiHttpMessageHandler>()
            .AddSingleton<PixivImageHttpMessageHandler>()
            .AddKeyedSingleton<HttpClient, MakoHttpClient>(MakoApiKind.AppApi,
                (s, _) => new(s.GetRequiredService<PixivApiHttpMessageHandler>())
                {
                    BaseAddress = new Uri(MakoHttpOptions.AppApiBaseUrl)
                })
            .AddKeyedSingleton<HttpClient, MakoHttpClient>(MakoApiKind.WebApi,
                (s, _) => new(s.GetRequiredService<PixivApiHttpMessageHandler>())
                {
                    BaseAddress = new Uri(MakoHttpOptions.WebApiBaseUrl),
                })
            .AddKeyedSingleton<HttpClient, MakoHttpClient>(MakoApiKind.AuthApi,
                (s, _) => new(s.GetRequiredService<PixivApiHttpMessageHandler>())
                {
                    BaseAddress = new Uri(MakoHttpOptions.OAuthBaseUrl)
                })
            .AddKeyedSingleton<HttpClient, MakoHttpClient>(MakoApiKind.ImageApi,
                (s, _) => new(s.GetRequiredService<PixivImageHttpMessageHandler>())
                {
                    DefaultRequestHeaders =
                    {
                        Referrer = new Uri("https://www.pixiv.net"),
                        UserAgent = { new("PixivIOSApp", "5.8.7") }
                    }
                })
            .AddSingleton<PixivTokenProvider>(t => new(t, firstTokenResponse))
            // .AddLogging(t => t.AddDebug())
            .AddWebApiClient()
            .UseSourceGeneratorHttpApiActivator()
            .ConfigureHttpApi(t => t.PrependJsonSerializerContext(AppJsonSerializerContext.Default));

        _ = serviceCollection.AddHttpApi<IAppApiEndPoint>()
            .ConfigurePrimaryHttpMessageHandler<PixivApiHttpMessageHandler>()
            .AddStandardResilienceHandler();
        _ = serviceCollection.AddHttpApi<IAuthEndPoint>()
            .ConfigurePrimaryHttpMessageHandler<PixivApiHttpMessageHandler>()
            .AddStandardResilienceHandler();
        _ = serviceCollection.AddHttpApi<IReverseSearchApiEndPoint>()
            .AddStandardResilienceHandler();

        return serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Cancels this <see cref="MakoClient" />, including all the running instances, the
    /// <see cref="MakoClient" /> will unable to be used again after calling this method
    /// </summary>
    public void Cancel()
    {
        _runningInstances.ForEach(instance => instance.EngineHandle.Cancel());
    }

    /// <summary>
    /// Ensures the current instances hasn't been cancelled
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureNotCancelled()
    {
        if (IsCancelled)
        {
            ThrowHelper.InvalidOperation($"MakoClient({Id}) has been cancelled");
        }
    }

    /// <summary>
    /// registers an instance to the running instances list
    /// </summary>
    /// <param name="engineHandleSource"></param>
    private void RegisterInstance(IEngineHandleSource engineHandleSource)
    {
        _runningInstances.Add(engineHandleSource);
    }

    /// <summary>
    /// removes an instance from the running instances list
    /// </summary>
    /// <param name="handle"></param>
    private void CancelInstance(EngineHandle handle)
    {
        _ = _runningInstances.RemoveAll(instance => instance.EngineHandle == handle);
    }

    /// <summary>
    /// <see cref="PrivacyPolicy.Private"/> is only allowed when the uid is pointing to yourself
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="privacyPolicy"></param>
    /// <returns></returns>
    private void CheckPrivacyPolicy(long uid, PrivacyPolicy privacyPolicy)
    {
        if (privacyPolicy is PrivacyPolicy.Private && Me.Id != uid)
            ThrowHelper.Throw(new IllegalPrivatePolicyException(uid));
    }

    /// <summary>
    /// Gets a registered <see cref="IFetchEngine{E}" /> by its <see cref="EngineHandle" />
    /// </summary>
    /// <param name="handle">The <see cref="EngineHandle" /> of the <see cref="IFetchEngine{E}" /></param>
    /// <typeparam name="T">The type of the results of the <see cref="IFetchEngine{E}" /></typeparam>
    /// <returns>The <see cref="IFetchEngine{E}" /> instance</returns>
    public IFetchEngine<T>? GetByHandle<T>(EngineHandle handle)
    {
        return _runningInstances.FirstOrDefault(h => h.EngineHandle == handle) as IFetchEngine<T>;
    }

    /// <summary>
    /// Acquires a configured <see cref="HttpClient" /> for the network traffics
    /// </summary>
    /// <param name="makoApiKind">The kind of API that is going to be used by the request</param>
    /// <returns>The <see cref="HttpClient" /> corresponding to <paramref name="makoApiKind" /></returns>
    public HttpClient GetMakoHttpClient(MakoApiKind makoApiKind)
    {
        return Provider.GetRequiredKeyedService<HttpClient>(makoApiKind);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        Dispose(Services);
        await Provider.DisposeAsync();
    }

    public void Dispose()
    {
        IsCancelled = true;
        GC.SuppressFinalize(this);
        Dispose(Services);
        Provider.Dispose();
    }

    private void Dispose(ServiceCollection collection)
    {
        foreach (var item in collection)
            if ((item.IsKeyedService
                    ? item.KeyedImplementationInstance
                    : item.ImplementationInstance)
                is IDisposable d && !Equals(d))
                d.Dispose();
    }

    public string Platform => IPlatformInfo.Pixiv;

    public HttpClient GetApiClient() => GetMakoHttpClient(MakoApiKind.AppApi);

    public HttpClient GetImageDownloadClient() => GetMakoHttpClient(MakoApiKind.ImageApi);
}
