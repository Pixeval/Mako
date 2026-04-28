// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Mako.Net;

internal sealed class MakoHttpMessageInvokerProvider(
    MakoClient makoClient,
    IServiceProvider serviceProvider) : IDisposable
{
    private static readonly Uri[] _ProxyProbeUris =
    [
        new(MakoHttpOptions.AppApiBaseUrl),
        new(MakoHttpOptions.OAuthBaseUrl),
        new($"https://{MakoHttpOptions.ImageHost}")
    ];

    private readonly Lock _gate = new();
    private readonly Dictionary<DomainFrontingType, HttpMessageInvoker> _domainFrontingInvokers = [];
    private readonly Dictionary<string, HttpMessageInvoker> _directInvokers = [];
    private HttpMessageInvoker? _imageDomainFrontingInvoker;
    private bool _disposed;

    public HttpMessageInvoker GetApiDomainFrontingInvoker(DomainFrontingType domainFrontingType)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_domainFrontingInvokers.TryGetValue(domainFrontingType, out var invoker))
                return invoker;

            var handler = serviceProvider.GetRequiredKeyedService<HttpMessageHandler>(domainFrontingType);
            invoker = new(handler, disposeHandler: false);
            _domainFrontingInvokers.Add(domainFrontingType, invoker);
            return invoker;
        }
    }

    public HttpMessageInvoker GetImageDomainFrontingInvoker()
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            return _imageDomainFrontingInvoker ??= new(new SocketsHttpHandler
            {
                ConnectCallback = makoClient.DomainFrontingConnectCallback,
                UseProxy = false
            });
        }
    }

    public HttpMessageInvoker GetDirectInvoker()
    {
        var proxySetting = makoClient.Configuration.Proxy;
        var proxy = makoClient.GetConfiguredProxy(proxySetting);
        var cacheKey = GetProxyCacheKey(proxySetting, proxy);

        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_directInvokers.TryGetValue(cacheKey, out var invoker))
                return invoker;

            invoker = new(new SocketsHttpHandler
            {
                UseProxy = proxy is not null,
                Proxy = proxy
            });
            _directInvokers.Add(cacheKey, invoker);
            return invoker;
        }
    }

    private static string GetProxyCacheKey(string? proxySetting, IWebProxy? proxy)
    {
        return proxySetting switch
        {
            null => "disabled",
            "" => proxy is null
                ? "system:"
                : $"system:{string.Join("|", _ProxyProbeUris.Select(uri => GetProxyCacheKeyPart(proxy, uri)))}",
            _ => $"explicit:{proxySetting}"
        };
    }

    private static string GetProxyCacheKeyPart(IWebProxy proxy, Uri uri)
    {
        try
        {
            // 包含代理本身和代理绕过两部分
            return $"{uri.AbsoluteUri}:{proxy.IsBypassed(uri)}:{proxy.GetProxy(uri)?.AbsoluteUri}";
        }
        catch (Exception e)
        {
            return $"{uri.AbsoluteUri}:{e.GetType().FullName}";
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (var invoker in _domainFrontingInvokers.Values)
                invoker.Dispose();

            foreach (var invoker in _directInvokers.Values)
                invoker.Dispose();

            _imageDomainFrontingInvoker?.Dispose();

            _domainFrontingInvokers.Clear();
            _directInvokers.Clear();
            _imageDomainFrontingInvoker = null;
        }
    }
}
