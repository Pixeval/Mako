// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Mako.Utilities;
using Misaki;

namespace Mako.Model;

// ReSharper disable UnusedAutoPropertyAccessor.Global
[Factory]
public partial record Illustration : WorkBase, IArtworkInfo
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<IllustrationType>))]
    public required IllustrationType Type { get; set; }

    [JsonPropertyName("tools")]
    public required string[] Tools { get; set; } = [];

    [JsonPropertyName("page_count")]
    public required long PageCount { get; set; }

    [JsonPropertyName("width")]
    public required int Width { get; set; }

    [JsonPropertyName("height")]
    public required int Height { get; set; }

    [JsonPropertyName("sanity_level")]
    public required long SanityLevel { get; set; }

    [JsonPropertyName("meta_single_page")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public required MetaSinglePage MetaSinglePage { get; set; }

    public string? OriginalSingleUrl => MetaSinglePage.OriginalImageUrl;

    [JsonPropertyName("meta_pages")]
    public required MetaPage[] MetaPages { get; set; } = [];

    [JsonPropertyName("illust_ai_type")]
    public override required AiType AiType { get; set; }

    [JsonPropertyName("illust_book_style")]
    public required int IllustBookStyle { get; set; }

    [MemberNotNullWhen(true, nameof(OriginalSingleUrl))]
    public bool IsUgoira => Type is IllustrationType.Ugoira;

    [MemberNotNullWhen(false, nameof(OriginalSingleUrl))]
    public bool IsManga => PageCount > 1;

    [JsonPropertyName("restriction_attributes")]
    public string? RestrictionAttributes { get; set; }

    public IReadOnlyList<string> MangaOriginalUrls => MetaPages.Select(m => m.ImageUrls.Original).ToArray();

    public List<string> GetUgoiraOriginalUrls(int frameCount)
    {
        Debug.Assert(IsUgoira);
        var list = new List<string>();
        for (var i = 0; i < frameCount; ++i)
            list.Add(OriginalSingleUrl.Replace("ugoira0", $"ugoira{i}"));
        return list;
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return Id.GetHashCode();
    }

    public virtual bool Equals(Illustration? other)
    {
        return other?.Id == Id;
    }

    DateTimeOffset IArtworkInfo.UpdateDate => CreateDate;

    DateTimeOffset IArtworkInfo.ModifyDate => CreateDate;

    IPreloadableEnumerable<IUser> IArtworkInfo.Authors => [User];

    IPreloadableEnumerable<IUser> IArtworkInfo.Uploaders => [];

    SafeRating IArtworkInfo.SafeRating => _safeRating;

    ILookup<ITagCategory, ITag> IArtworkInfo.Tags => Tags.ToLookup(_ => ITagCategory.Empty, ITag (t) => t);

    IReadOnlyList<IImageFrame> IArtworkInfo.Thumbnails => _thumbnails;

    IReadOnlyDictionary<string, object> IArtworkInfo.AdditionalInfo => _additionalInfo;
}

[Factory]
public partial record MetaSinglePage
{
    /// <summary>
    /// 单图或多图时的原图链接
    /// </summary>
    [JsonPropertyName("original_image_url")]
    public string? OriginalImageUrl { get; set; } = DefaultImageUrls.ImageNotAvailable;
}

[Factory]
public partial record MangaImageUrls : ImageUrls
{
    /// <summary>
    /// 多图时的原图链接
    /// </summary>
    [JsonPropertyName("original")]
    public required string Original { get; set; } = DefaultImageUrls.ImageNotAvailable;
}

[Factory]
public partial record MetaPage
{
    [JsonPropertyName("image_urls")]
    public required MangaImageUrls ImageUrls { get; set; }

    public string SquareMediumUrl => ImageUrls.SquareMedium;

    public string MediumUrl => ImageUrls.Medium;

    public string LargeUrl => ImageUrls.Large;

    /// <inheritdoc cref="MangaImageUrls.Original"/>
    public string OriginalUrl => ImageUrls.Original;
}
