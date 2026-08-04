// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mako.Net;

internal sealed class PixivAppApiHttpMessageHandler(
    MakoClient makoClient,
    PixivAppApiRequestThrottleState throttleState,
    MakoHttpMessageInvokerProvider invokerProvider)
    : MakoClientSupportedHttpMessageHandler(makoClient, invokerProvider)
{
    private static readonly TimeSpan _DefaultRateLimitCooldown = TimeSpan.FromMinutes(1);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Debug.Assert(request.RequestUri is { Host: MakoHttpOptions.AppApiHost });
        Debug.Assert(request.Headers.Authorization is not null);

        await throttleState.CooldownLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var delay = throttleState.CooldownUntil - DateTimeOffset.UtcNow;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

            var result = await SendApiAsync(request, cancellationToken).ConfigureAwait(false);
            var now = DateTimeOffset.UtcNow;
            throttleState.ExtendCooldown(now.AddMilliseconds(MakoClient.Configuration.ApiRequestCooldown));

            if (result.StatusCode is HttpStatusCode.TooManyRequests)
            {
                // Pixiv currently omits Retry-After, but honor it if the API starts returning one.
                var retryAt = result.Headers.RetryAfter switch
                {
                    { Delta: { } retryAfter } when retryAfter > TimeSpan.Zero => now.Add(retryAfter),
                    { Date: { } retryDate } when retryDate > now => retryDate,
                    _ => now.Add(_DefaultRateLimitCooldown)
                };
                throttleState.ExtendCooldown(retryAt);
                MakoClient.OnRateLimitEncountered(throttleState.CooldownUntil);
            }

            return result;
        }
        finally
        {
            throttleState.CooldownLock.Release();
        }
    }
}
