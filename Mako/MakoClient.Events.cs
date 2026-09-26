// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Net;
using System.Net.Http;
using Mako.Model;
using Mako.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Mako;

public partial class MakoClient
{
    public event EventHandler<MakoClient, Exception>? TokenRefreshedFailed;

    public event EventHandler<MakoClient, TokenResponse?>? TokenRefreshed;

    public event EventHandler<MakoClient, RateLimitEventArgs>? RateLimitEncountered;

    public DateTimeOffset AppApiRetryAt => Provider.GetRequiredService<PixivAppApiRequestThrottleState>().RateLimitUntil;

    internal void ReportRateLimit(HttpResponseMessage response, Uri? requestUri)
    {
        if (response.StatusCode is not HttpStatusCode.TooManyRequests)
            return;

        var now = DateTimeOffset.UtcNow;
        var retryAt = response.Headers.RetryAfter switch
        {
            { Delta: { } delay } when delay > TimeSpan.Zero => now.Add(delay),
            { Date: { } date } when date > now => date,
            _ => now.AddMinutes(1)
        };
        if (requestUri?.Host is MakoHttpOptions.AppApiHost)
        {
            var throttle = Provider.GetRequiredService<PixivAppApiRequestThrottleState>();
            throttle.ExtendRateLimit(retryAt);
            retryAt = throttle.RateLimitUntil;
        }
        OnRateLimitEncountered(retryAt);
    }

    internal void OnTokenRefreshedFailed(Exception e)
    {
        TokenRefreshedFailed?.Invoke(this, e);
    }

    internal void OnTokenRefreshed(TokenResponse? response)
    {
        TokenRefreshed?.Invoke(this, response);
    }

    internal void OnRateLimitEncountered(DateTimeOffset retryAt)
    {
        RateLimitEncountered?.Invoke(this, new(retryAt));
    }
}
