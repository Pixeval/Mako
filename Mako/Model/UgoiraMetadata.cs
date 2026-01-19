// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record UgoiraMetadata
{
    [JsonPropertyName("zip_urls")]
    public required ZipUrls ZipUrls { get; set; }

    [JsonPropertyName("frames")]
    public required IReadOnlyList<Frame> Frames { get; set; } = [];

    [field: AllowNull, MaybeNull]
    public IReadOnlyList<int> Delays => field ??= [..Frames.Select(t => t.Delay)];

    /// <inheritdoc cref="ZipUrls.Medium"/>
    public string MediumUrl => ZipUrls.Medium;

    /// <inheritdoc cref="ZipUrls.Large"/>
    public string LargeUrl => ZipUrls.Large;

    public string[] GetUgoiraOriginalUrls(string originalSingleUrl)
    {
        var arr = new string[Frames.Count];
        for (var i = 0; i < Frames.Count; ++i)
            arr[i] = originalSingleUrl.Replace("ugoira0", $"ugoira{i}");
        return arr;
    }

    public (Uri, int)[] GetUgoiraOriginalUrlsAndMsDelays(string originalSingleUrl)
    {
        var arr = new (Uri, int)[Frames.Count];
        for (var i = 0; i < Frames.Count; ++i)
            arr[i] = (new(originalSingleUrl.Replace("ugoira0", $"ugoira{i}")), Frames[i].Delay);
        return arr;
    }
}
