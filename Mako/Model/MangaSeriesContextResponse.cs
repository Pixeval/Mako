// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record MangaSeriesContextResponse
{
    [JsonPropertyName("illust_series_detail")]
    public required MangaSeriesDetail Detail { get; set; }

    [JsonPropertyName("illust_series_context")]
    public required MangaSeriesContext Context { get; set; }
}

[Factory]
public partial record MangaSeriesContext
{
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
