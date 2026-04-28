// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mako.Utilities;

namespace Mako.Net;

internal class PixivApiHttpMessageHandler(
    MakoClient makoClient,
    PixivApiRequestThrottleState throttleState,
    MakoHttpMessageInvokerProvider invokerProvider)
    : MakoClientSupportedHttpMessageHandler(makoClient, invokerProvider)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await throttleState.CooldownLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // 检查冷却时间并等待
            var delay = throttleState.Cooldown - DateTime.UtcNow;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

            var headers = request.Headers;
            var host = request.RequestUri!.Host; // the 'RequestUri' is guaranteed to be notnull here, because the 'HttpClient' will set it to 'BaseAddress' if it's null

            var configuration = MakoClient.Configuration;
            var domainFronting = (MakoHttpOptions.DomainFrontingRequiredHost.IsMatch(host) || host is MakoHttpOptions.OAuthHost) && configuration.DomainFronting;
            var domainFrontingType = configuration.DomainFrontingType;
            var userAgent = configuration.UserAgent;
            var cultureName = configuration.CultureInfo.Name;
            var cookie = configuration.Cookie;
            var cooldown = configuration.ApiRequestCooldown;

            headers.UserAgent.AddRange(userAgent);
            headers.AcceptLanguage.Add(new(cultureName));

            switch (host)
            {
                case MakoHttpOptions.AppApiHost:
                    Debug.Assert(headers.Authorization is not null);
                    break;
                case MakoHttpOptions.WebApiHost:
                    _ = headers.TryAddWithoutValidation("Cookie", cookie);
                    break;
            }

            var invoker = domainFronting
                ? InvokerProvider.GetApiDomainFrontingInvoker(domainFrontingType)
                : InvokerProvider.GetDirectInvoker();

            var result = await invoker
                .SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            // 更新冷却时间
            throttleState.Cooldown = DateTime.UtcNow.AddMilliseconds(cooldown);

            if (result.StatusCode is HttpStatusCode.TooManyRequests)
                MakoClient.OnRateLimitEncountered();

            return result;
        }
        finally
        {
            throttleState.CooldownLock.Release();
        }
    }
}
