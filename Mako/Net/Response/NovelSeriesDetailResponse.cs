// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
internal partial record NovelSeriesDetailResponse : NovelResponse
{
    [JsonPropertyName("novel_series_detail")]
    public required NovelSeriesDetail SeriesDetail { get; set; }

    [JsonPropertyName("novel_series_first_novel")]
    public required Novel First { get; set; }

    [JsonPropertyName("novel_series_latest_novel")]
    public required Novel Latest { get; set; }
}
