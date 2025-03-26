// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Text.Json.Serialization;
using Misaki;

namespace Mako.Model;

public record Spotlight : IIdentityInfo
{
    string IIdentityInfo.Platform => IIdentityInfo.Pixiv;

    [JsonPropertyName("id")]
    public required long Identity { get; set; }

    public string Id => Identity.ToString();

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
    [JsonConverter(typeof(JsonStringEnumConverter<SpotlightCategory>))]
    public required SpotlightCategory Category { get; set; }

    [JsonPropertyName("subcategory_label")]
    public required string SubcategoryLabel { get; set; }
}
