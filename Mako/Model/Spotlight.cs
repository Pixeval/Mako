// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Mako.Model;

public record Spotlight : IIdEntry
{
    [JsonPropertyName("id")]
    public required long Id { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("pure_title")]
    public required string PureTitle { get; set; }

    [JsonPropertyName("thumbnail")]
    public required string Thumbnail { get; set; } = DefaultImageUrls.ImageNotAvailable;

    [JsonPropertyName("article_url")]
    public required string ArticleUrl { get; set; }

    [JsonPropertyName("publish_date")]
    public required DateTimeOffset PublishDate { get; set; }

    [JsonPropertyName("category")]
    [JsonConverter(typeof(SnakeCaseLowerEnumConverter<SpotlightCategory>))]
    public required SpotlightCategory Category { get; set; }

    [JsonPropertyName("subcategory_label")]
    public required string SubcategoryLabel { get; set; }

    [field: AllowNull, MaybeNull]
    [JsonIgnore]
    public Uri WebsiteUri => field ??= new($"https://www.pixivision.net/a/{Id}");

    [field: AllowNull, MaybeNull]
    [JsonIgnore]
    public Uri AppUri => field ??= new($"pixeval://spotlight/{Id}");
}
