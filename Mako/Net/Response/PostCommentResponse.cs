// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Mako.Model;

namespace Mako.Net.Response;

public class PostCommentResponse : ISingleResultResponse<Comment>
{
    [JsonPropertyName("comment")]
    public required Comment Content { get; set; }
}
