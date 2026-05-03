// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Mako.Global.Enum;
using Mako.Net;
using Pixeval.Network.Maho.Ech;

namespace Mako;

/// <summary>
/// 
/// </summary>
/// <param name="DomainFronting"></param>
/// <param name="DomainFrontingType"></param>
/// <param name="Proxy"><see langword="null"/> to disable proxy, <see cref="string.Empty"/> to use system proxy, otherwise use the specified proxy</param>
/// <param name="Cookie"></param>
/// <param name="MirrorHost"></param>
/// <param name="ApiRequestCooldown"></param>
/// <param name="CultureInfo"></param>
public record MakoConfiguration(
    bool DomainFronting,
    DomainFrontingType DomainFrontingType,
    string? Proxy,
    string? Cookie,
    string? MirrorHost,
    TargetFilter TargetFilter,
    int ApiRequestCooldown,
    CultureInfo CultureInfo) : INativeInteropDnsResolver
{
    public MakoConfiguration() : this(false, DomainFrontingType.Fragmentation, "", "", "", TargetFilter.ForAndroid, 800, CultureInfo.CurrentCulture) { }

    public CultureInfo CultureInfo { get; set; } = CultureInfo;

    public IReadOnlyList<ProductInfoHeaderValue> UserAgent { get; set; } =
    [
        new("Mozilla", "5.0"),
        new("(Windows NT 10.0; Win64; x64)"),
        new("AppleWebKit", "537.36"),
        new("(KHTML, like Gecko)"),
        new("Chrome", "133.0.0.0"),
        new("Safari", "537.36"),
        new("Edg", "133.0.0.0")
    ];

    public bool DomainFronting { get; set; } = DomainFronting;

    public DomainFrontingType DomainFrontingType { get; set; } = DomainFrontingType;

    /// <summary>
    /// <see langword="null"/> to disable proxy, <see cref="string.Empty"/> to use system proxy, otherwise use the specified proxy
    /// </summary>
    public string? Proxy { get; set; } = Proxy;

    public string? Cookie { get; set; } = Cookie;

    public int ApiRequestCooldown { get; set; } = ApiRequestCooldown;

    /// <summary>
    /// Mirror server's host of image downloading
    /// </summary>
    public string? MirrorHost { get; set; } = MirrorHost;

    public TargetFilter TargetFilter { get; set; } = TargetFilter;

    public Dictionary<string, IPAddress[]> NameResolvers { get; } = new()
    {
        [MakoHttpOptions.ImageHost] = [],
        [MakoHttpOptions.WebApiHost] = [],
        [MakoHttpOptions.AccountHost] = [],
        [MakoHttpOptions.AppApiHost] = [],
        [MakoHttpOptions.ImageHost2] = [],
        [MakoHttpOptions.OAuthHost] = []
    };

    /// <inheritdoc />
    public string BaseResolutionUrl => "https://dns.alidns.com/resolve?name=cloudflare-ech.com&type=HTTPS";

    /// <inheritdoc />
    public Task<IPAddress[]> LookupAsync(string hostname) =>
        NameResolvers.TryGetValue(hostname, out var ips)
            ? Task.FromResult(ips)
            : Dns.GetHostAddressesAsync(hostname);
}
