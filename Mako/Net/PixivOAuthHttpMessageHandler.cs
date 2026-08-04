// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mako.Net;

internal sealed class PixivOAuthHttpMessageHandler(
    MakoClient makoClient,
    MakoHttpMessageInvokerProvider invokerProvider)
    : MakoClientSupportedHttpMessageHandler(makoClient, invokerProvider)
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Debug.Assert(request.RequestUri is { Host: MakoHttpOptions.OAuthHost });
        return SendApiAsync(request, cancellationToken);
    }
}
