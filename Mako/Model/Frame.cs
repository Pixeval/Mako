// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record Frame
{
    [JsonPropertyName("file")]
    public required string File { get; set; } = "";

    [JsonPropertyName("delay")]
    public required int Delay { get; set; }
}
