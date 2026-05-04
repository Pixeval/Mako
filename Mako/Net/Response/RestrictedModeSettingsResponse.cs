// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
internal partial record RestrictedModeSettingsResponse : ISingleResultResponse<bool>
{
    [JsonPropertyName("is_restricted_mode_enabled")]
    public required bool Content { get; set; }
}
