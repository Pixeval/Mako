// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Net;
using System.Net.Http;
using Mako.Global;

namespace Mako.Net;

public abstract class MakoClientSupportedHttpMessageHandler(MakoClient makoClient) : HttpMessageHandler, IMakoClientSupport
{
    public MakoClient MakoClient { get; } = makoClient;

    private HttpMessageInvoker? _domainFrontingInvoker;
    private HttpMessageInvoker? _directInvoker;
    private IWebProxy? _lastCachedProxy;

    public HttpMessageInvoker GetHttpMessageInvoker(bool domainFronting)
    {
        if (domainFronting)
        {
            _domainFrontingInvoker ??= MakoHttpOptions.CreateHttpMessageInvoker();
            return _domainFrontingInvoker;
        }

        var currentProxy = MakoClient.CurrentSystemProxy;
        // Create on first call or recreate if proxy has changed
        if (_directInvoker is null || _lastCachedProxy != currentProxy)
        {
            _directInvoker?.Dispose();
            _directInvoker = MakoHttpOptions.CreateDirectHttpMessageInvoker(MakoClient);
            _lastCachedProxy = currentProxy;
        }

        return _directInvoker;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _domainFrontingInvoker?.Dispose();
            _directInvoker?.Dispose();
        }

        base.Dispose(disposing);
    }
}
