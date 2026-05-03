// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record TrendingTagResponse : ISingleResultResponse<IReadOnlyList<TrendingTag>>
{
    [JsonPropertyName("trend_tags")]
    public required IReadOnlyList<TrendingTag> Content { get; set; } = [];
}
