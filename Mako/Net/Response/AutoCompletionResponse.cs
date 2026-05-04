// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
internal partial record AutoCompletionResponse : ISingleResultResponse<IReadOnlyList<Tag>>
{
    [JsonPropertyName("tags")]
    public required IReadOnlyList<Tag> Content { get; set; } = [];
}
