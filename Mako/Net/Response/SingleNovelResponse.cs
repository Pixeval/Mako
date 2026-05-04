// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[DebuggerDisplay("{Content}")]
[Factory]
internal partial record SingleNovelResponse : ISingleResultResponse<Novel>
{
    [JsonPropertyName("novel")]
    public required Novel Content { get; set; }
}
