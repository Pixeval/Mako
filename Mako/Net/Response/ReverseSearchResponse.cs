// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record ReverseSearchResponse
{
    [JsonPropertyName("header")]
    public required ReverseSearchResponseHeader Header { get; set; }

    [JsonPropertyName("results")]
    public required IReadOnlyList<Result> Results { get; set; } = [];
}

[Factory]
public partial record ReverseSearchResponseHeader
{
    [JsonPropertyName("status")]
    public required long Status { get; set; }
}

[Factory]
public partial record Result
{
    [JsonPropertyName("header")]
    public required ResultHeader Header { get; set; }

    [JsonPropertyName("data")]
    public required Data Data { get; set; }
}

[Factory]
public partial record Data
{
    [JsonPropertyName("title")]
    public required string Title { get; set; } = "";

    [JsonPropertyName("pixiv_id")]
    public required long PixivId { get; set; }

    [JsonPropertyName("member_name")]
    public required string MemberName { get; set; } = "";

    [JsonPropertyName("member_id")]
    public required long MemberId { get; set; }
}

[Factory]
public partial record ResultHeader
{
    [JsonPropertyName("similarity")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required double Similarity { get; set; }

    [JsonPropertyName("index_id")]
    public required long IndexId { get; set; }
}
