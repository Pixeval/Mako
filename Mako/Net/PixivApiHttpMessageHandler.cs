// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Mako.Global.Exception;
using Mako.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Mako.Net;

internal class PixivApiHttpMessageHandler(MakoClient makoClient) : MakoClientSupportedHttpMessageHandler(makoClient)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var headers = request.Headers;
        var host = request.RequestUri!.Host; // the 'RequestUri' is guaranteed to be notnull here, because the 'HttpClient' will set it to 'BaseAddress' if it's null

        var domainFronting = (MakoHttpOptions.DomainFrontingRequiredHost.IsMatch(host) || host is MakoHttpOptions.OAuthHost) && MakoClient.Configuration.DomainFronting;

        if (domainFronting)
            MakoHttpOptions.UseHttpScheme(request);

        headers.UserAgent.AddRange(MakoClient.Configuration.UserAgent);
        headers.AcceptLanguage.Add(new(MakoClient.Configuration.CultureInfo.Name));

        switch (host)
        {
            case MakoHttpOptions.AppApiHost:
                var provider = MakoClient.Provider.GetRequiredService<PixivTokenProvider>();
                var tokenResponse = await provider.GetTokenAsync().ConfigureAwait(false);
                if (tokenResponse is null)
                    throw new MakoTokenRefreshFailedException();
                headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                break;
            case MakoHttpOptions.WebApiHost:
                _ = headers.TryAddWithoutValidation("Cookie", MakoClient.Configuration.Cookie);
                break;
        }

        var result = await GetHttpMessageInvoker(domainFronting)
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (result.StatusCode is HttpStatusCode.TooManyRequests)
            MakoClient.OnRateLimitEncountered();

        return result;
    }
}
