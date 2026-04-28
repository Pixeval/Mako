// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mako.Utilities;

namespace Mako.Net;

internal class PixivImageHttpMessageHandler(
    MakoClient makoClient,
    MakoHttpMessageInvokerProvider invokerProvider)
    : MakoClientSupportedHttpMessageHandler(makoClient, invokerProvider)
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {

        var configuration = MakoClient.Configuration;
        var domainFronting = configuration.DomainFronting;
        var userAgent = configuration.UserAgent;
        var mirrorHost = configuration.MirrorHost;

        request.Headers.UserAgent.AddRange(userAgent);

        if (request.RequestUri is { Host: MakoHttpOptions.ImageHost } requestUri
            && mirrorHost is { } mirror
            && !string.IsNullOrWhiteSpace(mirror))
        {
            request.RequestUri = mirror switch
            {
                _ when Uri.CheckHostName(mirror) is not UriHostNameType.Unknown => new UriBuilder(requestUri) { Host = mirror }.Uri,
                _ when Uri.IsWellFormedUriString(mirror, UriKind.Absolute) => new Uri(mirror).Let(mirrorUri => new UriBuilder(requestUri) { Host = mirrorUri.Host, Scheme = mirrorUri.Scheme }).Uri,
                _ => throw new UriFormatException("Expecting a valid Host or URI")
            };
        }

        var invoker = domainFronting
            ? InvokerProvider.GetImageDomainFrontingInvoker()
            : InvokerProvider.GetDirectInvoker();

        if (domainFronting && request.RequestUri is not null)
            request.RequestUri = new UriBuilder(request.RequestUri) { Scheme = "http" }.Uri;

        return invoker.SendAsync(request, cancellationToken);
    }
}
