// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Net.Request;

public record RestrictedModeSettingsRequest([property: JsonPropertyName("is_restricted_mode_enabled")] bool IsRestrictedModeEnabled);
