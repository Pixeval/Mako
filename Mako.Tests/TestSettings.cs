using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Mako.Global.Enum;
using Mako.Net;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: DoNotParallelize]

namespace Mako.Tests;

[TestClass]
public static class TestSettings
{
    public static string RefreshToken => throw new NotImplementedException();

    public static IReadOnlyList<string> PixivAppApiNameResolver { get; } =
    [
        "104.18.42.239",
        "172.64.145.17"
    ];

    public static IReadOnlyList<string> PixivWebApiNameResolver { get; } =
    [
        "210.140.139.155",
        "210.140.139.156",
        "210.140.139.157"
    ];

    public static IReadOnlyList<string> PixivAccountNameResolver { get; } =
    [
        "210.140.139.155",
        "210.140.139.156",
        "210.140.139.157"
    ];

    public static IReadOnlyList<string> PixivOAuthNameResolver { get; } =
    [
        "104.18.42.239",
        "172.64.145.17"
    ];

    public static IReadOnlyList<string> PixivImageNameResolver { get; } =
    [
        "210.140.139.134",
        "210.140.139.135",
        "210.140.139.136",
        "210.140.139.137"
    ];

    public static IReadOnlyList<string> PixivImageNameResolver2 { get; } =
    [
        "210.140.139.135",
        "210.140.139.136",
        "210.140.139.137"
    ];

    public static MakoClient Client { get; private set; } = null!;

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext _)
    {
        var conf = new MakoConfiguration(
            DomainFronting: false,
            DomainFrontingType: DomainFrontingType.Fragmentation,
            Proxy: "" /*UseSystemProxy*/,
            Cookie: null,
            MirrorHost: null,
            TargetFilter: TargetFilter.ForAndroid,
            ApiRequestCooldown: 800,
            CultureInfo: CultureInfo.CurrentUICulture)
        {
            NameResolvers =
            {
                [MakoHttpOptions.AppApiHost] = [.. PixivAppApiNameResolver.Select(IPAddress.Parse)],
                [MakoHttpOptions.ImageHost] = [.. PixivImageNameResolver.Select(IPAddress.Parse)],
                [MakoHttpOptions.ImageHost2] = [.. PixivImageNameResolver2.Select(IPAddress.Parse)],
                [MakoHttpOptions.OAuthHost] = [.. PixivOAuthNameResolver.Select(IPAddress.Parse)],
                [MakoHttpOptions.AccountHost] = [.. PixivAccountNameResolver.Select(IPAddress.Parse)],
                [MakoHttpOptions.WebApiHost] = [.. PixivWebApiNameResolver.Select(IPAddress.Parse)],
            }
        };

        using var loggerProvider = new DebugLoggerProvider();
        var logger = loggerProvider.CreateLogger(nameof(MakoClient));
        Client = new(conf, logger);
        Client.SetToken(RefreshToken);
    }
}

