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

internal class PixivApiHttpMessageHandler : MakoClientSupportedHttpMessageHandler
{
    public PixivApiHttpMessageHandler(MakoClient makoClient) : base(makoClient)
    {
        // PixivApiHttpMessageHandler should be singleton
        Debug.Assert(!_SingletonFlag);
        _SingletonFlag = true;
    }

    private static bool _SingletonFlag;

    private DateTime Cooldown { get; set; }
    
    private readonly SemaphoreSlim _cooldownLock = new(1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await _cooldownLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // 检查冷却时间并等待
            var delay = Cooldown - DateTime.UtcNow;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

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
                    Debug.Assert(headers.Authorization is not null);
                    break;
                case MakoHttpOptions.WebApiHost:
                    _ = headers.TryAddWithoutValidation("Cookie", MakoClient.Configuration.Cookie);
                    break;
            }

            var result = await GetHttpMessageInvoker(domainFronting)
                .SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            // 更新冷却时间
            Cooldown = DateTime.UtcNow.AddMilliseconds(MakoClient.Configuration.ApiRequestCooldown);

            if (result.StatusCode is HttpStatusCode.TooManyRequests)
                MakoClient.OnRateLimitEncountered();

            return result;
        }
        finally
        {
            _cooldownLock.Release();
        }
    }
}
