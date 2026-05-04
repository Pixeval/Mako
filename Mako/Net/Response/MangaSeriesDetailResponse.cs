// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
internal partial record MangaSeriesDetailResponse : IllustrationResponse
{
    [JsonPropertyName("illust_series_detail")]
    public required MangaSeriesDetail SeriesDetail { get; set; }

    [JsonPropertyName("illust_series_first_illust")]
    public required Illustration First { get; set; }
}
