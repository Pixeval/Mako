// Copyright (c) Mako.
// Licensed under the MIT License.

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
    [JsonPropertyName("pixiv_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public long? PixivId { get; set; }

    [JsonPropertyName("danbooru_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public long? DanbooruId { get; set; }

    [JsonPropertyName("yandere_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public long? YandereId { get; set; }

    [JsonPropertyName("gelbooru_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public long? GelbooruId { get; set; }

    [JsonPropertyName("sankaku_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public long? SankakuId { get; set; }
}

[Factory]
public partial record ResultHeader
{
    [JsonPropertyName("similarity")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required double Similarity { get; set; }

    [JsonPropertyName("index_id")]
    public required IndexType IndexId { get; set; }
}

public enum IndexType
{
    Pixiv = 5,
    PixivHistorical = 6,
    Danbooru = 9,
    Yandere = 12,
    Gelbooru = 25,
    Sankaku = 27
}
