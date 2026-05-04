// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
internal partial record WebApiBookmarksWithTagResponse
{
    [JsonPropertyName("body")]
    public required WebApiBookmarksWithTagBody ResponseBody { get; set; }
}

[Factory]
internal partial record WebApiBookmarksWithTagBody
{
    [JsonPropertyName("works")]
    public required IReadOnlyList<Work> Works { get; set; } = [];
}

[Factory]
internal partial record Work
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required long Id { get; set; }
}
