// Copyright (c) Mako.
// Licensed under the GPL v3 License.

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Misaki;

namespace Mako.Model;

[DebuggerDisplay("{Id}: {Title} [{User}]")]
public abstract record WorkBase : IWorkEntry
{
    string IIdentityInfo.Platform => IIdentityInfo.Pixiv;

    [JsonPropertyName("id")]
    public required long Id { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; } = "";

    [JsonPropertyName("caption")]
    public required string Description { get; set; } = "";

    [JsonPropertyName("restrict")]
    [JsonConverter(typeof(BoolToNumberJsonConverter))]
    public required bool IsPrivate { get; set; }

    public abstract AiType AiType { get; set; }

    [JsonPropertyName("x_restrict")]
    public required XRestrict XRestrict { get; set; }

    [JsonPropertyName("tags")]
    public required Tag[] Tags { get; set; } = [];

    [JsonPropertyName("user")]
    public required UserInfo User { get; set; }

    [JsonPropertyName("create_date")]
    public required DateTimeOffset CreateDate { get; set; }

    [JsonPropertyName("image_urls")]
    public required ImageUrls ThumbnailUrls { get; set; }

    [JsonPropertyName("is_bookmarked")]
    public required bool IsBookmarked { get; set; }

    [JsonPropertyName("total_bookmarks")]
    public required int TotalFavorite { get; set; }

    [JsonPropertyName("total_view")]
    public required int TotalView { get; set; }

    [JsonPropertyName("total_comments")]
    public required int TotalComments { get; set; }

    [JsonPropertyName("visible")]
    public required bool Visible { get; set; }

    [JsonPropertyName("is_muted")]
    public required bool IsMuted { get; set; }

    [JsonPropertyName("series")]
    public required Series? Series
    {
        get;
        set
        {
            if (value != _defaultSeries)
                field = value;
        }
    }

    private readonly Series _defaultSeries = new();
}

internal class BoolToNumberJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetInt32() is not 0;

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteNumberValue(value ? 1 : 0);
}
