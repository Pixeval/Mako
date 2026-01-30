// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mako.Engine;
using Mako.Global.Enum;
using Mako.Global.Exception;
using Mako.Model;
using Mako.Net;
using Mako.Net.EndPoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Misaki;

namespace Mako;

public partial class MakoClient : IDisposable, IAsyncDisposable, IDownloadHttpClientService, IGetArtworkService, IPostFavoriteService
{
    /// <summary>
    /// Create a new <see cref="MakoClient" /> based on given <see cref="Configuration" />, <see cref="TokenResponse" />
    /// </summary>
    /// <param name="configuration">The <see cref="Configuration" /></param>
    /// <param name="logger"></param>
    public MakoClient(MakoConfiguration configuration, ILogger logger)
    {
        Logger = logger;
        Configuration = configuration;
        Provider = BuildServiceProvider(Services);
    }

    /// <summary>
    /// Injects necessary dependencies
    /// </summary>
    /// <returns>The <see cref="ServiceProvider" /> contains all the required dependencies</returns>
    private ServiceProvider BuildServiceProvider(IServiceCollection serviceCollection)
    {
        _ = serviceCollection
            .AddSingleton(this)
            .AddSingleton<PixivApiHttpMessageHandler>()
            .AddSingleton<PixivImageHttpMessageHandler>()
            .AddSingleton<RefreshTokenOption>();

        _ = serviceCollection.AddHttpApi<IAuthEndPoint>()
            .ConfigurePrimaryHttpMessageHandler<PixivApiHttpMessageHandler>();

        _ = serviceCollection.AddHttpApi<IAppApiEndPoint>()
            .ConfigurePrimaryHttpMessageHandler<PixivApiHttpMessageHandler>();

        _ = serviceCollection.AddTokenProvider<IAppApiEndPoint, PixivTokenProvider>();

        _ = serviceCollection.AddHttpClient(nameof(MakoApiKind.ImageApi))
            .ConfigureHttpClient(client =>
            {
                client.DefaultRequestHeaders.Referrer = new Uri("https://www.pixiv.net");
                client.DefaultRequestHeaders.UserAgent.Add(new("PixivIOSApp", "5.8.7"));
            })
            .ConfigurePrimaryHttpMessageHandler<PixivImageHttpMessageHandler>();

        _ = serviceCollection
            .AddHttpClient(nameof(MakoApiKind.AppApi))
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(MakoHttpOptions.AppApiBaseUrl))
            .ConfigurePrimaryHttpMessageHandler<PixivApiHttpMessageHandler>();

        _ = serviceCollection
            .AddHttpClient(nameof(MakoApiKind.WebApi))
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(MakoHttpOptions.WebApiBaseUrl))
            .ConfigurePrimaryHttpMessageHandler<PixivApiHttpMessageHandler>();

        _ = serviceCollection
            .AddHttpClient(nameof(MakoApiKind.AuthApi))
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(MakoHttpOptions.OAuthBaseUrl))
            .ConfigurePrimaryHttpMessageHandler<PixivApiHttpMessageHandler>();

        _ = serviceCollection
            .AddWebApiClient()
            .UseSourceGeneratorHttpApiActivator()
            .ConfigureHttpApi(t => t.PrependJsonSerializerContext(MakoJsonSerializerContext.Default));

        _ = serviceCollection.AddHttpApi<IReverseSearchApiEndPoint>();

        return serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Cancels all the running instances
    /// </summary>
    public void CancelAll()
    {
        EnsureBuilt();
        _runningInstances.ForEach(instance => instance.EngineHandle.Cancel());
    }

    /// <summary>
    /// registers an instance to the running instances list
    /// </summary>
    /// <param name="engineHandleSource"></param>
    private void RegisterInstance(IEngineHandleSource engineHandleSource)
    {
        EnsureBuilt();
        _runningInstances.Add(engineHandleSource);
    }

    /// <summary>
    /// removes an instance from the running instances list
    /// </summary>
    /// <param name="handle"></param>
    private void CancelInstance(EngineHandle handle)
    {
        EnsureBuilt();
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
        EnsureBuilt();
        if (privacyPolicy is PrivacyPolicy.Private && Me?.Id != uid)
            throw new IllegalPrivatePolicyException(uid);
    }

    /// <summary>
    /// Gets a registered <see cref="IFetchEngine{E}" /> by its <see cref="EngineHandle" />
    /// </summary>
    /// <param name="handle">The <see cref="EngineHandle" /> of the <see cref="IFetchEngine{E}" /></param>
    /// <typeparam name="T">The type of the results of the <see cref="IFetchEngine{E}" /></typeparam>
    /// <returns>The <see cref="IFetchEngine{E}" /> instance</returns>
    public IFetchEngine<T>? GetByHandle<T>(EngineHandle handle)
    {
        EnsureBuilt();
        return _runningInstances.FirstOrDefault(h => h.EngineHandle == handle) as IFetchEngine<T>;
    }

    /// <summary>
    /// Acquires a configured <see cref="HttpClient" /> for the network traffics
    /// </summary>
    /// <param name="makoApiKind">The kind of API that is going to be used by the request</param>
    /// <returns>The <see cref="HttpClient" /> corresponding to <paramref name="makoApiKind" /></returns>
    public HttpClient GetMakoHttpClient(MakoApiKind makoApiKind)
    {
        EnsureBuilt();
        var factory = Provider.GetRequiredService<IHttpClientFactory>();
        return factory.CreateClient(makoApiKind.ToString());
    }

    public HttpClient GetApiClient() => GetMakoHttpClient(MakoApiKind.AppApi);

    public HttpClient GetImageDownloadClient() => GetMakoHttpClient(MakoApiKind.ImageApi);

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if (Status is not ClientStatus.Built)
            return;
        Status = ClientStatus.Disposed;
        Dispose(Services);
        await Provider.DisposeAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (Status is not ClientStatus.Built)
            return;
        Status = ClientStatus.Disposed;
        Dispose(Services);
        Provider.Dispose();
    }

    private void EnsureBuilt()
    {
        ObjectDisposedException.ThrowIf(Status is ClientStatus.Disposed, this);
    }

    private void Dispose(ServiceCollection collection)
    {
        CancelAll();
        foreach (var item in collection)
            if ((item.IsKeyedService
                    ? item.KeyedImplementationInstance
                    : item.ImplementationInstance)
                is IDisposable d && !Equals(d))
                d.Dispose();
    }
}
