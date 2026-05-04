// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
internal partial record UgoiraMetadataResponse : ISingleResultResponse<UgoiraMetadata>
{
    [JsonPropertyName("ugoira_metadata")]
    public required UgoiraMetadata Content { get; set; }
}
