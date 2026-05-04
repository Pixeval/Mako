// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System;
using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

public abstract record SeriesDetailBase : SimpleSeries
{
    [JsonPropertyName("user")]
    public required UserInfo User { get; set; }

    [JsonPropertyName("caption")]
    public required string Caption { get; set; } = "";

    [JsonPropertyName("watchlist_added")]
    public required bool WatchlistAdded { get; set; }
}

[Factory]
public partial record NovelSeriesDetail : SeriesDetailBase
{
    [JsonPropertyName("is_original")]
    public required bool IsOriginal { get; set; }

    [JsonPropertyName("is_concluded")]
    public required bool IsConcluded { get; set; }

    [JsonPropertyName("content_count")]
    public required int ContentCount { get; set; }

    [JsonPropertyName("total_character_count")]
    public required int TotalCharacterCount { get; set; }

    [JsonPropertyName("display_text")]
    public required string DisplayText { get; set; } = "";

    [JsonPropertyName("novel_ai_type")]
    public required AiType NovelAiType { get; set; }
}

[Factory]
public partial record MangaSeriesDetail : SeriesDetailBase
{
    [JsonPropertyName("cover_image_urls")]
    public required MediumOnlyImageUrl CoverImageUrls { get; set; }

    [JsonPropertyName("series_work_count")]
    public required int SeriesWorkCount { get; set; }

    [JsonPropertyName("create_date")]
    public required DateTimeOffset CreateDate { get; set; }

    [JsonPropertyName("width")]
    public required int Width { get; set; }

    [JsonPropertyName("height")]
    public required int Height { get; set; }
}
