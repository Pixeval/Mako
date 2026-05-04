// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
internal partial record ShowAiSettingsResponse : ISingleResultResponse<bool>
{
    [JsonPropertyName("show_ai")]
    public required bool Content { get; set; }
}
