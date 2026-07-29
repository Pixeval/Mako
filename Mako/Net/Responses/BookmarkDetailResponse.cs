// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Responses;

[Factory]
internal partial record BookmarkDetailResponse : ISingleResultResponse<BookmarkDetail>
{
    [JsonPropertyName("bookmark_detail")]
    public required BookmarkDetail Content { get; set; }
}
