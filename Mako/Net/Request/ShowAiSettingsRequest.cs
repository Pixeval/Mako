// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Net.Request;

public record ShowAiSettingsRequest([property: JsonPropertyName("show_ai")] bool ShowAi);
