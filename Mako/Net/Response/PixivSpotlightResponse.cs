// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record PixivSpotlightResponse : IPixivNextUrlResponse<Spotlight>
{
    [JsonPropertyName("next_url")]
    public required string? NextUrl { get; set; }

    [JsonPropertyName("spotlight_articles")]
    public /*override*/ required Spotlight[] Entities { get; set; } = [];
}
