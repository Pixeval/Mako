// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;

namespace Mako;

/// <summary>
/// Contains all the user-configurable keys
/// </summary>
public record MakoConfiguration(
    bool DomainFronting,
    string? Proxy,
    string? Cookie,
    string? MirrorHost,
    int ApiRequestCooldown,
    CultureInfo CultureInfo)
{
    public MakoConfiguration() : this(false, "", "", "", 800, CultureInfo.CurrentCulture) { }

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

    public string? Proxy { get; set; } = Proxy;

    public string? Cookie { get; set; } = Cookie;

    public int ApiRequestCooldown { get; set; } = ApiRequestCooldown;

    /// <summary>
    /// Mirror server's host of image downloading
    /// </summary>
    public string? MirrorHost { get; set; } = MirrorHost;
}
