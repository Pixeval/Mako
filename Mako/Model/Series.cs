// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record Series : SimpleSeries
{
    [JsonPropertyName("user")]
    public required UserInfo User { get; set; }

    [JsonPropertyName("mask_text")]
    public required string? MaskText { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; } = "";

    [JsonPropertyName("published_content_count")]
    public required int PublishedContentCount { get; set; }

    [JsonPropertyName("latest_content_id")]
    public required long LatestContentId { get; set; }

    [JsonPropertyName("last_published_content_datetime")]
    public required DateTimeOffset LastPublishedContentDatetime { get; set; }
}
