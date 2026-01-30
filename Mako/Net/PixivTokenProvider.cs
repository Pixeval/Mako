// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Mako.Net.EndPoints;
using Mako.Net.Request;
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace Mako.Net;

internal class RefreshTokenOption
{
    public string? RefreshToken { get; set; }
}

internal class PixivTokenProvider(IServiceProvider serviceProvider) : TokenProvider(serviceProvider)
{
    private MakoClient MakoClient { get; } = serviceProvider.GetRequiredService<MakoClient>();

    /// <inheritdoc />
    protected override async Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider)
    {
        var refreshToken = serviceProvider.GetRequiredService<RefreshTokenOption>().RefreshToken;

        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var tokenResponse = await serviceProvider.GetRequiredService<IAuthEndPoint>().RefreshAsync(new RefreshSessionRequest(refreshToken)).ConfigureAwait(false);

        MakoClient.SetTokenInternal(tokenResponse);

        return new()
        {
            Access_token = tokenResponse.AccessToken,
            Expires_in = tokenResponse.ExpiresIn,
            Token_type = tokenResponse.TokenType is "bearer" ? "Bearer" : tokenResponse.TokenType,
            Refresh_token = tokenResponse.RefreshToken
        };
    }

    /// <inheritdoc />
    protected override Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refreshToken) => RequestTokenAsync(serviceProvider);
}
