//// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Mako.Utilities;
using Misaki;

namespace Mako.Model;

// ReSharper disable UnusedAutoPropertyAccessor.Global
[Factory]
public partial record Illustration : WorkBase, IWorkEntry, ISingleImage, ISingleAnimatedImage, IImageSet
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<IllustrationType>))]
    public required IllustrationType Type { get; set; }

    [JsonPropertyName("tools")]
    public required IReadOnlyList<string> Tools { get; set; } = [];

    [JsonPropertyName("page_count")]
    public required int PageCount { get; set; }

    [JsonPropertyName("width")]
    public required int Width { get; set; }

    [JsonPropertyName("height")]
    public required int Height { get; set; }

    [JsonPropertyName("sanity_level")]
    public required int SanityLevel { get; set; }

    [JsonPropertyName("meta_single_page")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public required MetaSinglePage MetaSinglePage { get; set; }

    public string? OriginalSingleUrl => MetaSinglePage.OriginalImageUrl;

    [JsonPropertyName("meta_pages")]
    public required IReadOnlyList<MetaPage> MetaPages { get; set; } = [];

    [JsonPropertyName("illust_ai_type")]
    public required AiType AiType { get; set; }

    [JsonPropertyName("illust_book_style")]
    public required int IllustBookStyle { get; set; }

    [MemberNotNullWhen(true, nameof(OriginalSingleUrl))]
    public bool IsUgoira => Type is IllustrationType.Ugoira;

    [MemberNotNullWhen(false, nameof(OriginalSingleUrl))]
    public bool IsManga => PageCount > 1;

    /// <remarks>
    /// ["restricted_mode"]
    /// </remarks>
    [JsonPropertyName("restriction_attributes")]
    public IReadOnlyList<string>? RestrictionAttributes { get; set; }

    public IReadOnlyList<string> MangaOriginalUrls => [.. MetaPages.Select(m => m.ImageUrls.Original)];

    public List<string> GetUgoiraOriginalUrls(int frameCount)
    {
        Debug.Assert(IsUgoira);
        var list = new List<string>();
        for (var i = 0; i < frameCount; ++i)
            list.Add(OriginalSingleUrl.Replace("ugoira0", $"ugoira{i}"));
        return list;
    }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Id.GetHashCode();

    public virtual bool Equals(Illustration? other) => other?.Id == Id;

    public Uri WebsiteUri => new($"https://www.pixiv.net/artworks/{Id}");

    public Uri AppUri => new($"pixeval://illust/{Id}");

    DateTimeOffset IArtworkInfo.UpdateDate => CreateDate;

    DateTimeOffset IArtworkInfo.ModifyDate => CreateDate;

    IPreloadableEnumerable<IUser> IArtworkInfo.Authors => [User];

    IPreloadableEnumerable<IUser> IArtworkInfo.Uploaders => [];

    SafeRating IArtworkInfo.SafeRating => XRestrict switch
    {
        XRestrict.R18 => SafeRating.Explicit,
        XRestrict.R18G => SafeRating.Guro,
        XRestrict.Ordinary => SanityLevel switch
        {
            6 => SafeRating.Questionable,
            4 or 2 when RestrictionAttributes is not null => SafeRating.Sensitive,
            2 => SafeRating.General,
            _ => SafeRating.NotSpecified
        },
        _ => SafeRating.NotSpecified
    };

    ILookup<ITagCategory, ITag> IArtworkInfo.Tags => Tags.ToLookup(_ => ITagCategory.Empty, ITag (t) => t);

    [field: AllowNull, MaybeNull]
    IReadOnlyCollection<IImageFrame> IArtworkInfo.Thumbnails => field ??=
        [
            GetImageFrame(ThumbnailSize.C540X540Q70, ImageFrame.FixType.FixHeight) with { ImageUri = new(ThumbnailUrls.Medium) },
            GetImageFrame(ThumbnailSize.C600X1200Q90, ImageFrame.FixType.FixWidth) with { ImageUri = new(ThumbnailUrls.Large) }
        ];

    private IReadOnlyList<ImageFrame> GetImageFrames()
    {
        var list = new List<ImageFrame>();
        foreach (var value in ((ThumbnailSize[]) typeof(ThumbnailSize).GetEnumValues()))
        {
            var fieldInfo = typeof(ThumbnailSize).GetField(value.ToString());
            if (fieldInfo?.GetCustomAttributes<ImageFrame.FixTypeAttribute>() is not { } arr)
                continue;

            list.AddRange(arr.Select(fixTypeAttribute => GetImageFrame(value, fixTypeAttribute.FixType)));
        }

        return list;
    }

    private ImageFrame GetImageFrame(ThumbnailSize value, ImageFrame.FixType fixType)
    {
        var fieldInfo = typeof(ThumbnailSize).GetField(value.ToString());
        if (fieldInfo?.GetCustomAttribute<ImageFrame.SizeAttribute>() is not
                { Width: var width, Height: var height })
            return ThrowHelper.NotSupported<ImageFrame>(value.ToString());
        var uri = new Uri(GetThumbnail(value, isSquare: fixType is ImageFrame.FixType.FixAll));

        return fixType switch
        {
            ImageFrame.FixType.FixHeight => new ImageFrame(this, height, false) { ImageUri = uri },
            ImageFrame.FixType.FixWidth => new ImageFrame(this, width, true) { ImageUri = uri },
            ImageFrame.FixType.FixAll => new ImageFrame(width, height) { ImageUri = uri },
            _ => ThrowHelper.NotSupported<ImageFrame>(fixType.ToString())
        };
    }

    IReadOnlyDictionary<string, object> IArtworkInfo.AdditionalInfo => new Dictionary<string, object>();

    public ImageType ImageType => IsManga
        ? ImageType.ImageSet
        : IsUgoira
            ? ImageType.SingleAnimatedImage
            : ImageType.SingleImage;

    public bool IsAiGenerated => AiType is AiType.AiGenerated;

    public string GetThumbnail(ThumbnailSize size = ThumbnailSize.C540X540Q70, int page = 0, bool isSquare = false) => $"https://i.pximg.net{size.GetDescription()}/img-master/img/{CreateDate:yyyy/MM/dd/HH/mm/ss}/{Id}_p{page}_{(isSquare ? "square" : "master")}1200.jpg";

    public string GetOriginal(int page = 0) => $"https://i.pximg.net/img-original/img/{CreateDate:yyyy/MM/dd/HH/mm/ss}/{Id}_p{page}.jpg";

    ulong IImageFrame.ByteSize => 0;

    Uri IImageFrame.ImageUri => new(GetOriginal());

    public int SetIndex { get; private init; }

    [field: AllowNull, MaybeNull]
    IPreloadableEnumerable<IArtworkInfo> IImageSet.Pages => field ??=
    [
        ..PageCount <= 1
            ? [this]
            // The API result of manga (a work with multiple illustrations) is a single Illustration object
            // that only differs from the illustrations of a single work on the MetaPages property, this property
            // contains the download urls of the manga
            : MetaPages.Select((m, i) => this with
            {
                MetaSinglePage = new() { OriginalImageUrl = m.ImageUrls.Original },
                ThumbnailUrls = m.ImageUrls,
                SetIndex = i
            })
    ];

    public enum ThumbnailSize
    {
        /// <summary>
        /// (official square_medium) (height prior) standard square
        /// </summary>
        [Description("/c/360x360_70")]
        [ImageFrame.Size(360, 360)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C360X360Q70,

        /// <summary>
        /// (official medium) (height prior) standard square
        /// </summary>
        [Description("/c/540x540_70")]
        [ImageFrame.Size(540, 540)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C540X540Q70,

        /// <summary>
        /// (official large) (width prior) standard
        /// </summary>
        [Description("/c/600x1200_90")]
        [ImageFrame.Size(600, 1200)]
        [ImageFrame.FixType(ImageFrame.FixType.FixWidth)]
        C600X1200Q90,

        /// <summary>
        /// (height prior) standard square
        /// </summary>
        [Description("/c/100x100")]
        [ImageFrame.Size(100, 100)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C100X100,

        /// <summary>
        /// (height prior) standard square
        /// </summary>
        [Description("/c/128x128")]
        [ImageFrame.Size(128, 128)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C128X128,

        /// <summary>
        /// (height prior) standard square
        /// </summary>
        [Description("/c/150x150")]
        [ImageFrame.Size(150, 150)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C150X150,

        /// <summary>
        /// (height prior) standard square
        /// </summary>
        [Description("/c/240x240")]
        [ImageFrame.Size(240, 240)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C240X240,

        /// <summary>
        /// (width prior) standard
        /// </summary>
        [Description("/c/240x480")]
        [ImageFrame.Size(240, 480)]
        [ImageFrame.FixType(ImageFrame.FixType.FixWidth)]
        C240X480,

        /// <summary>
        /// square
        /// </summary>
        [Description("/c/250x250_80_a2")]
        [ImageFrame.Size(250, 250)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C250X250Q80A2,

        /// <summary>
        /// (height prior) standard square
        /// </summary>
        [Description("/c/260x260_80")]
        [ImageFrame.Size(260, 260)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C260X260Q80,

        /// <summary>
        /// square
        /// </summary>
        [Description("/c/288x288_80_a2")]
        [ImageFrame.Size(288, 288)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C288X288Q80A2,

        /// <summary>
        /// (height prior) standard
        /// </summary>
        [Description("/c/400x250_80")]
        [ImageFrame.Size(400, 250)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        C400X250Q80,

        /// <summary>
        /// (height prior) standard
        /// </summary>
        [Description("/c/600x600")]
        [ImageFrame.Size(600, 600)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        C600X600,

        /// <summary>
        /// (width prior) standard
        /// </summary>
        [Description("/c/768x1200_80")]
        [ImageFrame.Size(768, 1200)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        C768X1200Q80,

        /// <summary>
        /// standard square (1200x1200)
        /// </summary>
        [Description("/")]
        [ImageFrame.Size(1200, 1200)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        C

        /*
        /// <summary>
        /// crop
        /// </summary>
        [Description("/c/300x200_a2")]
        [ImageFrame.Size(300, 200)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C300X200A2,

        /// <summary>
        /// (height prior) standard square webp
        /// </summary>
        [Description("/c/360x360_10_webp")]
        [ImageFrame.Size(360, 360)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C360X360Q10Webp,

        /// <summary>
        /// (height prior) standard square webp
        /// </summary>
        [Description("/c/540x540_10_webp")]
        [ImageFrame.Size(540, 540)]
        [ImageFrame.FixType(ImageFrame.FixType.FixHeight)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C540X540Q10Webp,

        /// <summary>
        /// (width prior) standard webp (square gives 600x600)
        /// </summary>
        [Description("/c/600x1200_90_webp")]
        [ImageFrame.Size(600, 600)]
        [ImageFrame.FixType(ImageFrame.FixType.FixWidth)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C600X1200Q90Webp,

        /// <summary>
        /// crop webp
        /// </summary>
        [Description("/c/1080x600_10_a2_u1_webp")]
        [ImageFrame.Size(1080, 600)]
        [ImageFrame.FixType(ImageFrame.FixType.FixAll)]
        C1080X600Q10A2U1Webp
        */
    }
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
