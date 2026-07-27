// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Responses;

[Factory]
internal partial record BookmarkUserInfoResponse : IPixivNextUrlResponse<BookmarkUserInfo>
{
    [JsonPropertyName("next_url")]
    public required string? NextUrl { get; set; }

    [JsonPropertyName("users")]
    public /*override*/ required IReadOnlyList<BookmarkUserInfo> Entities { get; set; } = [];
}
