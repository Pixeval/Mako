// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Mako.Model;
using Mako.Net.Request;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Mako.Net.EndPoints;

[Header(HttpRequestHeader.UserAgent, "PixivAndroidApp/6.140.2 (Android 15.0)")]
[Header(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded")]
[HttpHost(MakoHttpOptions.OAuthBaseUrl)]
public interface IAuthEndPoint
{
    [HttpPost("/auth/token")]
    Task<TokenResponse> RefreshAsync([FormContent] RefreshSessionRequest request);

    [HttpPost("/auth/token")]
    Task<TokenResponse> RequestAsync([FormContent] RequestSessionRequest request);
}
