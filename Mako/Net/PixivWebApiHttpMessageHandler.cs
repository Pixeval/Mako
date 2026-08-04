// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mako.Utilities;

namespace Mako.Net;

internal sealed class PixivWebApiHttpMessageHandler(
    MakoClient makoClient,
    MakoHttpMessageInvokerProvider invokerProvider)
    : MakoClientSupportedHttpMessageHandler(makoClient, invokerProvider)
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Debug.Assert(request.RequestUri is { Host: MakoHttpOptions.WebApiHost });

        var configuration = MakoClient.Configuration;
        request.Headers.UserAgent.AddRange(configuration.UserAgent);
        if (!string.IsNullOrWhiteSpace(configuration.Cookie))
            _ = request.Headers.TryAddWithoutValidation("Cookie", configuration.Cookie);

        return SendApiAsync(request, cancellationToken);
    }
}
