// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Text.Json.Serialization;

namespace Mako.Net.Requests;

public record RestrictedModeSettingsRequest([property: JsonPropertyName("is_restricted_mode_enabled")] bool IsRestrictedModeEnabled);
