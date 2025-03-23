// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Diagnostics;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[DebuggerDisplay("{Novel}")]
[Factory]
public partial record PixivSingleNovelResponse
{
    [JsonPropertyName("novel")]
    public required Novel Novel { get; set; }
}
