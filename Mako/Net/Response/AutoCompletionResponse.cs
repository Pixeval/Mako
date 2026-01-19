// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record AutoCompletionResponse
{
    [JsonPropertyName("tags")]
    public required IReadOnlyList<Tag> Tags { get; set; } = [];
}
