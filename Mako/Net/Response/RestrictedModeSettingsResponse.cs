// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record RestrictedModeSettingsResponse
{
    [JsonPropertyName("is_restricted_mode_enabled")]
    public required bool IsRestrictedModeEnabled { get; set; }
}
