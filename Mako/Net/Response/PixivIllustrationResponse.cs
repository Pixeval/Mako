// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record PixivIllustrationResponse : IPixivNextUrlResponse<Illustration>
{
    [JsonPropertyName("next_url")]
    public required string? NextUrl { get; set; }

    [JsonPropertyName("illusts")]
    public /*override*/ required IReadOnlyList<Illustration> Entities { get; set; } = [];
}
