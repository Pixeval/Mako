// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
internal partial record SpotlightResponse : IPixivNextUrlResponse<Spotlight>
{
    [JsonPropertyName("next_url")]
    public required string? NextUrl { get; set; }

    [JsonPropertyName("spotlight_articles")]
    public /*override*/ required IReadOnlyList<Spotlight> Entities { get; set; } = [];
}
