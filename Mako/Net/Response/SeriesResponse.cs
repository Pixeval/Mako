// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record SeriesResponse : IPixivNextUrlResponse<Series>
{
    [JsonPropertyName("next_url")]
    public required string? NextUrl { get; set; }

    [JsonPropertyName("series")]
    public /*override*/ required IReadOnlyList<Series> Entities { get; set; } = [];
}
