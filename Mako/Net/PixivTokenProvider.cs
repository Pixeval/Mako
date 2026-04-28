// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Mako.Model;
using Mako.Net.EndPoints;
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace Mako.Net;

internal class RefreshTokenOption
{
    public string? Code { get; set; }

    public string? CodeVerifier { get; set; }

    public string? RefreshToken { get; set; }
}

internal class PixivTokenProvider(IServiceProvider serviceProvider) : TokenProvider(serviceProvider)
{
    private MakoClient MakoClient { get; } = serviceProvider.GetRequiredService<MakoClient>();

    /// <inheritdoc />
    protected override async Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider)
    {
        var option = serviceProvider.GetRequiredService<RefreshTokenOption>();

        TokenResponse tokenResponse;
        if (string.IsNullOrWhiteSpace(option.Code) || string.IsNullOrWhiteSpace(option.CodeVerifier))
        {
            if (string.IsNullOrWhiteSpace(option.RefreshToken))
                return null;

            tokenResponse = await serviceProvider.GetRequiredService<IAuthEndPoint>()
                .RefreshAsync(new(option.RefreshToken)).ConfigureAwait(false);
        }
        else
        {
            tokenResponse = await serviceProvider.GetRequiredService<IAuthEndPoint>()
                .RequestAsync(new(option.Code, option.CodeVerifier)).ConfigureAwait(false);

            option.Code = null;
            option.CodeVerifier = null;
            option.RefreshToken = tokenResponse.RefreshToken;
        }

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
