// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Mako.Net.EndPoints;
using Mako.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mako;

public partial class MakoClient
{
    private Task<IReadOnlyList<T>> RunWithLoggerAsync<T>(Func<IAppApiEndPoint, Task<IReadOnlyList<T>>> task)
    {
        return RunWithLoggerAsync(() => task(Provider.GetRequiredService<IAppApiEndPoint>()), []);
    }

    private Task<bool> RunWithLoggerAsync(Func<IAppApiEndPoint, Task<bool>> task)
    {
        return RunWithLoggerAsync(() => task(Provider.GetRequiredService<IAppApiEndPoint>()), false);
    }

    private Task<IEnumerable<T>> RunWithLoggerAsync<T>(Func<IAppApiEndPoint, Task<IEnumerable<T>>> task)
    {
        return RunWithLoggerAsync(() => task(Provider.GetRequiredService<IAppApiEndPoint>()), []);
    }

    private Task<HttpResponseMessage> RunWithLoggerAsync(Func<IAppApiEndPoint, Task<HttpResponseMessage>> task)
    {
        return RunWithLoggerAsync(() => task(Provider.GetRequiredService<IAppApiEndPoint>()), new HttpResponseMessage(HttpStatusCode.RequestTimeout));
    }

    private async Task RunWithLoggerAsync(Func<IAppApiEndPoint, Task> task)
    {
        try
        {
            EnsureNotCancelled();
            await task(Provider.GetRequiredService<IAppApiEndPoint>());
        }
        catch (Exception e)
        {
            LogException(e);
        }
    }

    private Task<T> RunWithLoggerAsync<T>(Func<IAppApiEndPoint, Task<T>> task) where T : IDefaultFactory<T>
    {
        return RunWithLoggerAsync(() => task(Provider.GetRequiredService<IAppApiEndPoint>()), T.CreateDefault);
    }

    private Task<T> RunWithLoggerAsync<T>(Func<Task<T>> task) where T : IDefaultFactory<T>
    {
        return RunWithLoggerAsync(task, T.CreateDefault);
    }

    private Task<IReadOnlyList<T>> RunWithLoggerAsync<T>(Func<Task<IReadOnlyList<T>>> task) where T : IDefaultFactory<T>
    {
        return RunWithLoggerAsync(task, []);
    }

    private Task<HttpResponseMessage> RunWithLoggerAsync(Func<Task<HttpResponseMessage>> task)
    {
        return RunWithLoggerAsync(task, new HttpResponseMessage(HttpStatusCode.RequestTimeout));
    }

    private Task<T> RunWithLoggerAsync<T>(Func<Task<Result<T>>> task, Func<T> createDefault)
    {
        return RunWithLoggerAsync(async () =>
        {
            var result = await task();
            switch (result)
            {
                case Result<T>.Success { Value: var value }:
                    return value;
                case Result<T>.Failure { Cause: { } e }:
                    LogException(e);
                    return createDefault();
                default:
                    return ThrowHelper.ArgumentOutOfRange<Result<T>, T>(result);
            }
        }, createDefault);
    }

    private async Task<T> RunWithLoggerAsync<T>(Func<Task<T>> task, Func<T> createDefault)
    {
        try
        {
            EnsureNotCancelled();

            return await task();
        }
        catch (Exception e)
        {
            LogException(e);
            return createDefault();
        }
    }

    private async Task<T> RunWithLoggerAsync<T>(Func<Task<T>> task, T createDefault)
    {
        try
        {
            EnsureNotCancelled();

            return await task();
        }
        catch (Exception e)
        {
            LogException(e);
            return createDefault;
        }
    }

    private Task<T> RunWithLoggerAsync<T>(Func<Task<Result<T>>> task) where T : IDefaultFactory<T>
    {
        return RunWithLoggerAsync(task, T.CreateDefault);
    }

    internal void LogException(Exception e) => Logger.LogError(e, "MakoClient Exception");

    [DynamicDependency("ConstructSystemProxy", "SystemProxyInfo", "System.Net.Http")]
    static MakoClient()
    {
        var type = typeof(HttpClient).Assembly.GetType("System.Net.Http.SystemProxyInfo");
        var method = type?.GetMethod("ConstructSystemProxy");
        var @delegate = method?.CreateDelegate<Func<IWebProxy>>();

        _GetCurrentSystemProxy = @delegate ?? ThrowHelper.Throw<MissingMethodException, Func<IWebProxy>>(new("Unable to find proxy functions"));
        HttpClient.DefaultProxy = _GetCurrentSystemProxy();
    }

    private static readonly Func<IWebProxy> _GetCurrentSystemProxy;

    public IWebProxy? CurrentSystemProxy
    {
        get
        {
            switch (Configuration.Proxy)
            {
                case null:
                    return null;
                case "":
                {
                    var now = DateTime.Now;
                    if (now < CoolDown)
                        return HttpClient.DefaultProxy;
                    CoolDown = now.AddSeconds(2);
                    return HttpClient.DefaultProxy = _GetCurrentSystemProxy();
                }
                default:
                    return new WebProxy(Configuration.Proxy);
            }
        }
    }

    private static DateTime CoolDown { get; set; } = DateTime.Now.AddSeconds(2);
}
