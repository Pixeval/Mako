// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record NovelResponse : IPixivNextUrlResponse<Novel>
{
    [JsonPropertyName("next_url")]
    public required string? NextUrl { get; set; }

    [JsonPropertyName("novels")]
    public /*override*/ required IReadOnlyList<Novel> Entities { get; set; } = [];
}
