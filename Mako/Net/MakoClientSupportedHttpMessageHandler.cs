// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mako.Global;

namespace Mako.Net;

public abstract class MakoClientSupportedHttpMessageHandler : HttpMessageHandler, IMakoClientSupport
{
    internal MakoClientSupportedHttpMessageHandler(
        MakoClient makoClient,
        MakoHttpMessageInvokerProvider invokerProvider)
    {
        MakoClient = makoClient;
        InvokerProvider = invokerProvider;
    }

    public MakoClient MakoClient { get; }

    private protected MakoHttpMessageInvokerProvider InvokerProvider { get; }

    private protected Task<HttpResponseMessage> SendApiAsync(HttpRequestMessage request, CancellationToken token)
    {
        var configuration = MakoClient.Configuration;
        var invoker = configuration.DomainFronting
            ? InvokerProvider.GetApiDomainFrontingInvoker(configuration.DomainFrontingType)
            : InvokerProvider.GetDirectInvoker();
        return invoker.SendAsync(request, token);
    }
}
