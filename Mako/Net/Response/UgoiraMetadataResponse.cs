// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record UgoiraMetadataResponse
{
    [JsonPropertyName("ugoira_metadata")]
    public required UgoiraMetadata UgoiraMetadataInfo { get; set; }
}
