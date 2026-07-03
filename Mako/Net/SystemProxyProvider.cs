// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Mako.Net;

public static class SystemProxyProvider
{
    private static readonly Lock _Gate = new();

    private static DateTime _NextRefreshTime = DateTime.UtcNow.AddSeconds(2);

    static SystemProxyProvider()
    {
        HttpClient.DefaultProxy = ConstructSystemProxy();
    }

    public static IWebProxy GetCurrent()
    {
        lock (_Gate)
        {
            var now = DateTime.UtcNow;
            if (now < _NextRefreshTime)
                return HttpClient.DefaultProxy;

            _NextRefreshTime = now.AddSeconds(2);
            return HttpClient.DefaultProxy = ConstructSystemProxy();
        }
    }

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "ConstructSystemProxy")]
    private static extern IWebProxy ConstructSystemProxy([UnsafeAccessorType("System.Net.Http.SystemProxyInfo, System.Net.Http")] object? _ = null);
}
