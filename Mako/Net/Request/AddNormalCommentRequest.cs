// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Text.Json.Serialization;

namespace Mako.Net.Request;

public record AddNormalIllustCommentRequest(
    [property: JsonPropertyName("illust_id")] long Id,
    [property: JsonPropertyName("parent_comment_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] long? ParentCommentId,
    [property: JsonPropertyName("comment")] string Content
);

public record AddNormalNovelCommentRequest(
    [property: JsonPropertyName("novel_id")] long Id,
    [property: JsonPropertyName("parent_comment_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] long? ParentCommentId,
    [property: JsonPropertyName("comment")] string Content
);
