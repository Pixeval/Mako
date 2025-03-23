// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record ShowAiSettingsResponse
{
    [JsonPropertyName("show_ai")]
    public required bool ShowAi { get; set; }
}
