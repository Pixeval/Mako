// Copyright (c) Mako.Tests.
// Licensed under the GPL-3.0 License.

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mako.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mako.Tests;

[TestClass]
public sealed class RateLimitNotificationTest
{
    [TestMethod]
    public async Task HttpLayerNotifiesButDoesNotReplayRequest()
    {
        using var client = new MakoClient(new(), NullLogger.Instance);
        using var provider = client.Provider;
        using var transport = new RateLimitedTransport();
        using var inner = new HttpMessageInvoker(transport, false);
        using var handler = new ProbeHandler(client, inner);
        using var invoker = new HttpMessageInvoker(handler, false);
        using var request = new HttpRequestMessage(HttpMethod.Post, MakoHttpOptions.AppApiBaseUrl);
        var notifications = 0;
        client.RateLimitEncountered += (_, _) => notifications++;

        using var response = await invoker.SendAsync(request, CancellationToken.None);

        Assert.AreEqual(HttpStatusCode.TooManyRequests, response.StatusCode);
        Assert.AreEqual(1, transport.RequestCount);
        Assert.AreEqual(1, notifications);
        Assert.IsTrue(client.AppApiRetryAt > DateTimeOffset.UtcNow);
    }

    [TestMethod]
    [DataRow(MakoHttpOptions.AppApiHost)]
    [DataRow(MakoHttpOptions.WebApiHost)]
    [DataRow(MakoHttpOptions.OAuthHost)]
    [DataRow(MakoHttpOptions.ImageHost)]
    public void ReportsEveryApiKindWithoutApplyingAppCooldownToOtherKinds(string host)
    {
        using var client = new MakoClient(new(), NullLogger.Instance);
        using var provider = client.Provider;
        using var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        var retryAt = DateTimeOffset.UtcNow.AddMinutes(4);
        response.Headers.RetryAfter = new(retryAt);
        RateLimitEventArgs? notification = null;
        client.RateLimitEncountered += (_, args) => notification = args;

        client.ReportRateLimit(response, new Uri($"https://{host}/test"));

        Assert.IsNotNull(notification);
        Assert.AreEqual(retryAt, notification.RetryAt);
        Assert.AreEqual(host is MakoHttpOptions.AppApiHost ? retryAt : DateTimeOffset.MinValue, client.AppApiRetryAt);
    }

    [TestMethod]
    public void UsesFallbackAndDoesNotShortenExistingCooldown()
    {
        using var client = new MakoClient(new(), NullLogger.Instance);
        using var provider = client.Provider;
        using var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        var uri = new Uri(MakoHttpOptions.AppApiBaseUrl);
        var before = DateTimeOffset.UtcNow;
        client.ReportRateLimit(response, uri);
        Assert.IsTrue(client.AppApiRetryAt >= before.AddMinutes(1));
        Assert.IsTrue(client.AppApiRetryAt <= DateTimeOffset.UtcNow.AddMinutes(1));
        var first = client.AppApiRetryAt;

        response.Headers.RetryAfter = new(TimeSpan.FromSeconds(1));
        client.ReportRateLimit(response, uri);
        Assert.AreEqual(first, client.AppApiRetryAt);
    }

    [TestMethod]
    public void OtherStatusesDoNotNotifyOrThrottle()
    {
        using var client = new MakoClient(new(), NullLogger.Instance);
        using var provider = client.Provider;
        using var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        var count = 0;
        client.RateLimitEncountered += (_, _) => count++;
        client.ReportRateLimit(response, new Uri(MakoHttpOptions.AppApiBaseUrl));
        Assert.AreEqual(0, count);
        Assert.AreEqual(DateTimeOffset.MinValue, client.AppApiRetryAt);
    }

    private sealed class ProbeHandler(MakoClient client, HttpMessageInvoker invoker)
        : MakoClientSupportedHttpMessageHandler(client, client.Provider.GetRequiredService<MakoHttpMessageInvokerProvider>())
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            SendWithRateLimitNotificationAsync(invoker, request, cancellationToken);
    }

    private sealed class RateLimitedTransport : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.TooManyRequests));
        }
    }
}
