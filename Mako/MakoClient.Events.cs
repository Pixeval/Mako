// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using Mako.Model;

namespace Mako;

public partial class MakoClient
{
    public event EventHandler<MakoClient, Exception>? TokenRefreshedFailed;

    public event EventHandler<MakoClient, TokenResponse?>? TokenRefreshed;

    public event EventHandler<MakoClient, RateLimitEventArgs>? RateLimitEncountered;

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
