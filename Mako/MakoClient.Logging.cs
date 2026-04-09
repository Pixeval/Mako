// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
            EnsureBuilt();
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

    private async Task<T> RunWithLoggerAsync<T>(Func<Task<T>> task, Func<T> createDefault)
    {
        try
        {
            EnsureBuilt();

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
            EnsureBuilt();

            return await task();
        }
        catch (Exception e)
        {
            LogException(e);
            return createDefault;
        }
    }

    internal void LogException(Exception e) => Logger.LogError(e, "MakoClient Exception");

    static MakoClient()
    {
        HttpClient.DefaultProxy = GetSystemProxy();
    }

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "ConstructSystemProxy")]
    private static extern IWebProxy GetSystemProxy([UnsafeAccessorType("System.Net.Http.SystemProxyInfo, System.Net.Http")] object? _ = null);

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
                    var now = DateTime.UtcNow;
                    if (now < Cooldown)
                        return HttpClient.DefaultProxy;
                    Cooldown = now.AddSeconds(2);
                    return HttpClient.DefaultProxy = GetSystemProxy();
                }
                default:
                    return new WebProxy(Configuration.Proxy);
            }
        }
    }

    private static DateTime Cooldown { get; set; } = DateTime.UtcNow.AddSeconds(2);
}
