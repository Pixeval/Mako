// Copyright (c) Mako.Tests.
// Licensed under the GPL-3.0 License.

using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mako.Global.Enum;
using Mako.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mako.Tests;

[TestClass]
public sealed class ApiHttpMessageHandlerTest
{
    [TestMethod]
    [TestCategory("Integration")]
    public async Task TestAppApiCooldownDoesNotDelayWebApiAsync()
    {
        var configuration = new MakoConfiguration(
            DomainFronting: false,
            DomainFrontingType: DomainFrontingType.Fragmentation,
            Proxy: null,
            Cookie: TestSettings.Cookie,
            MirrorHost: null,
            TargetFilter: TargetFilter.ForAndroid,
            ApiRequestCooldown: 30_000,
            CultureInfo: CultureInfo.InvariantCulture);
        await using var client = new MakoClient(configuration, NullLogger.Instance);
        await client.SetTokenAsync(TestSettings.RefreshToken);

        using (var appApiTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
        {
            var tags = await client.GetWorkTrendingTagsAsync(SimpleWorkType.Illustration, appApiTimeout.Token);
            Assert.IsNotEmpty(tags);
        }

        using var webApiTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var webClient = client.GetMakoHttpClient(MakoApiKind.WebApi);
        using var response = await webClient.GetAsync("/stacc?mode=unify", webApiTimeout.Token);
        var content = await response.Content.ReadAsStringAsync(webApiTimeout.Token);

        Assert.IsTrue(response.IsSuccessStatusCode, $"Web feed returned HTTP {(int) response.StatusCode}.");
        StringAssert.Contains(content, "pixiv.stacc.env.preload.stacc");
    }
}
