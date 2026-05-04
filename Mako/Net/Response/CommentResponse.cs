// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
internal partial record CommentResponse : IPixivNextUrlResponse<Comment>
{
    [JsonPropertyName("next_url")]
    public required string? NextUrl { get; set; }

    [JsonPropertyName("comments")]
    public /*override*/ required IReadOnlyList<Comment> Entities { get; set; } = [];

    // 一级Comment会有此字段，二级Comment没有，只见过0值，不知道什么意思
    [JsonPropertyName("comment_access_control")]
    public int CommentAccessControl { get; set; }
}
