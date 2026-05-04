// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mako.Model;

[DebuggerDisplay("{Id}: {Title} [{User}]")]
public abstract record WorkBase
{
    [JsonPropertyName("id")]
    public required long Id { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; } = "";

    [JsonPropertyName("caption")]
    public required string Description { get; set; } = "";

    [JsonPropertyName("restrict")]
    [JsonConverter(typeof(BoolToNumberJsonConverter))]
    public required bool IsPrivate { get; set; }

    [JsonPropertyName("x_restrict")]
    public required XRestrict XRestrict { get; set; }

    [JsonPropertyName("tags")]
    public required IReadOnlyList<Tag> Tags { get; set; } = [];

    [JsonPropertyName("user")]
    public required UserInfo User { get; set; }

    [JsonPropertyName("create_date")]
    public required DateTimeOffset CreateDate { get; set; }

    [JsonPropertyName("image_urls")]
    public required ImageUrls ThumbnailUrls { get; set; }

    [JsonPropertyName("is_bookmarked")]
    public required bool IsFavorite { get; set; }

    [JsonPropertyName("total_bookmarks")]
    public required int TotalFavorite { get; set; }

    [JsonPropertyName("total_view")]
    public required int TotalView { get; set; }

    [JsonPropertyName("visible")]
    public required bool Visible { get; set; }

    [JsonPropertyName("is_muted")]
    public required bool IsMuted { get; set; }

    [JsonPropertyName("series")]
    public required SimpleSeries? Series
    {
        get;
        set
        {
            // 此属性为空时响应是一个空对象 {} 而非 null
            if (value != _DefaultSeries)
                field = value;
        }
    }

    private static readonly SimpleSeries _DefaultSeries = new();
}

public record SimpleSeries : IIdEntry
{
    // 虽然这两个字段不是required，但除了WorkBase其他情况都会有这两个字段
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";
}

internal class BoolToNumberJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetInt32() is not 0;

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteNumberValue(value ? 1 : 0);
}
