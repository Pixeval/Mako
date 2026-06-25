// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Text.Json.Serialization;

namespace Mako.Net.Requests;

public record AiShowSettingsRequest([property: JsonPropertyName("show_ai")] bool ShowAi);
