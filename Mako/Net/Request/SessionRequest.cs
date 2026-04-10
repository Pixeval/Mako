// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Net.Request;

public abstract class SessionRequestBase
{
    [JsonPropertyName("grant_type")]
    public abstract string GrantType { get; }

    [JsonPropertyName("client_id")]
    public string ClientId => "MOBrBDS8blbauoSck0ZfDbtuzpyT";

    [JsonPropertyName("client_secret")]
    public string ClientSecret => "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";

    [JsonPropertyName("include_policy")]
    public string IncludePolicy => "true";
}

public class RefreshSessionRequest(string refreshToken) : SessionRequestBase
{
    [JsonPropertyName("grant_type")]
    public override string GrantType => "refresh_token";

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; } = refreshToken;
}

public class RequestSessionRequest(string? code, string codeVerifier) : SessionRequestBase
{
    [JsonPropertyName("grant_type")]
    public override string GrantType => "authorization_code";

    [JsonPropertyName("code")]
    public string? Code { get; } = code;

    [JsonPropertyName("code_verifier")]
    public string? CodeVerifier { get; } = codeVerifier;

    [JsonPropertyName("redirect_uri")]
    public string RedirectUri => "https://app-api.pixiv.net/web/v1/users/auth/pixiv/callback";
}
