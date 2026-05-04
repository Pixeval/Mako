// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record MangaSeriesContext
{
    [JsonPropertyName("illust_series_detail")]
    public required MangaSeriesDetail SeriesDetail { get; set; }

    /// <summary>
    /// 1 based
    /// </summary>
    [JsonPropertyName("content_order")]
    public required int ContentOrder { get; set; }

    [JsonPropertyName("prev")]
    public required Illustration Previous { get; set; }

    [JsonPropertyName("next")]
    public required Illustration Next { get; set; }
}
