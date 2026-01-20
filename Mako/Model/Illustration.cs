// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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

    [JsonPropertyName("meta_pages")]
    public required IReadOnlyList<MetaPage> MetaPages { get; set; } = [];

    [JsonPropertyName("illust_ai_type")]
    public required AiType AiType { get; set; }

    [JsonPropertyName("illust_book_style")]
    public required int IllustBookStyle { get; set; }

    /// <remarks>
    /// ["restricted_mode"]
    /// </remarks>
    [JsonPropertyName("restriction_attributes")]
    public IReadOnlyList<string>? RestrictionAttributes { get; set; }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Id.GetHashCode();

    public virtual bool Equals(Illustration? other) => other?.Id == Id;

    #region 额外属性

    [JsonInclude]
    public UgoiraMetadata? UgoiraMetadata { get; internal set; }

    [JsonInclude]
    public int SetIndex { get; internal set; } = -1;

    #endregion

    #region 帮助类成员

    [JsonIgnore]
    public string? OriginalSingleUrl => MetaSinglePage.OriginalImageUrl;

    [MemberNotNullWhen(true, nameof(OriginalSingleUrl))]
    [JsonIgnore]
    public bool IsPicGif => Type is IllustrationType.Ugoira;

    [MemberNotNullWhen(false, nameof(OriginalSingleUrl))]
    [JsonIgnore]
    public bool IsPicSet => PageCount > 1 || SetIndex > -1;

    #endregion

    #region Misaki成员及其相关帮助类成员

    [field: AllowNull, MaybeNull]
    [JsonIgnore]
    public Uri WebsiteUri => field ??= new($"https://www.pixiv.net/artworks/{Id}");

    [field: AllowNull, MaybeNull]
    [JsonIgnore]
    public Uri AppUri => field ??= new($"pixeval://illust/{Id}");

    [JsonIgnore]
    IPreloadableList<IUser> IArtworkInfo.Authors => [User];

    [JsonIgnore]
    IPreloadableList<IUser> IArtworkInfo.Uploaders => [];

    [JsonIgnore]
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

    [JsonIgnore]
    ILookup<ITagCategory, ITag> IArtworkInfo.Tags => Tags.ToLookup(_ => ITagCategory.Empty, ITag (t) => t);

    [JsonIgnore]
    IReadOnlyCollection<IImageFrame> IArtworkInfo.Thumbnails =>
    [
        new ImageFrame(GetImageSize(ThumbnailSize.C540X540Q70)) { ImageUri = new(ThumbnailUrls.Medium) },
        new ImageFrame(GetImageSize(ThumbnailSize.C600X1200Q90)) { ImageUri = new(ThumbnailUrls.Large) },
        new ImageFrame(GetImageSize(ThumbnailSize.C)) { ImageUri = new(ThumbnailUrls.NotCropped) },
    ];

    private IImageSize GetImageSize(ThumbnailSize value)
    {
        var fieldInfo = typeof(ThumbnailSize).GetField(value.ToString());
        if (fieldInfo?.GetCustomAttribute<ImageFrame.SizeAttribute>() is not
                { Width: var width, Height: var height })
            throw new NotSupportedException(value.ToString());
        return IImageSize.Uniform(this, width, height);
    }

    [JsonIgnore]
    IReadOnlyDictionary<string, object> IArtworkInfo.AdditionalInfo => new Dictionary<string, object>();

    /// <summary>
    /// TODO: 也许可以给图集里的一页单独一个枚举？
    /// </summary>
    [JsonIgnore]
    public ImageType ImageType => IsPicSet
        ? ImageType.ImageSet
        : IsPicGif
            ? ImageType.SingleAnimatedImage
            : ImageType.SingleImage;

    [JsonIgnore]
    public bool IsAiGenerated => AiType is AiType.AiGenerated;

    [JsonIgnore]
    ulong IImageFrame.ByteSize => 0;

    [JsonIgnore]
    Uri IImageFrame.ImageUri => new(OriginalSingleUrl!);

    [JsonIgnore]
    public SingleAnimatedImageType PreferredAnimatedImageType => SingleAnimatedImageType.MultiFiles;

    [JsonIgnore]
    public Uri? SingleImageUri => null;

    [JsonIgnore]
    public IPreloadableList<int>? ZipImageDelays => null;

    [field: AllowNull, MaybeNull]
    [JsonIgnore]
    public IPreloadableList<(Uri, int)> MultiImageUris => field ??=
        PreloadableList.ToPreloadableList<(Uri, int), UgoiraMetadata>(
            UgoiraMetadata,
            GetUgoiraMetadataAsync,
            ugoiraMetadata => ugoiraMetadata.GetUgoiraOriginalUrlsAndMsDelays(OriginalSingleUrl!));

    [field: AllowNull, MaybeNull]
    [JsonIgnore]
    public IPreloadableList<IAnimatedImageFrame> AnimatedThumbnails => field ??=
        PreloadableList.ToPreloadableList<IAnimatedImageFrame, UgoiraMetadata>(
            UgoiraMetadata,
            GetUgoiraMetadataAsync,
            ugoiraMetadata =>
            [
                new AnimatedImageFrame(GetImageSize(ThumbnailSize.C540X540Q70),
                    new Uri(ugoiraMetadata.MediumUrl),
                    [.. ugoiraMetadata.Delays]),
                new AnimatedImageFrame(GetImageSize(ThumbnailSize.C600X1200Q90),
                    new Uri(ugoiraMetadata.LargeUrl),
                    [.. ugoiraMetadata.Delays]),
            ]);

    [MemberNotNull(nameof(UgoiraMetadata))]
    private async Task<UgoiraMetadata> GetUgoiraMetadataAsync(IMisakiService service)
    {
        if (!IsPicGif)
            throw new InvalidOperationException("Not Ugoira");
        if (service is not MakoClient makoClient)
            throw new InvalidOperationException("Invalid service");
        return UgoiraMetadata ??= await makoClient.GetUgoiraMetadataAsync(Id);
    }

    [field: AllowNull, MaybeNull]
    [JsonIgnore]
    IPreloadableList<ISingleImage> IImageSet.Pages => field ??=
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

    public string Serialize() => JsonSerializer.Serialize(this, typeof(Illustration), AppJsonSerializerContext.Default);

    public static ISerializable Deserialize(string data) => (Illustration) JsonSerializer.Deserialize(data, typeof(Illustration), AppJsonSerializerContext.Default)!;

    [JsonIgnore]
    public string SerializeKey => typeof(Illustration).FullName!;

    #endregion

    public enum ThumbnailSize
    {
        /// <summary>
        /// (official square_medium) standard square
        /// </summary>
        [Description("c/360x360_70/")]
        [ImageFrame.Size(360, 360)]
        C360X360Q70,

        /// <summary>
        /// (official medium) standard square
        /// </summary>
        [Description("c/540x540_70/")]
        [ImageFrame.Size(540, 540)]
        C540X540Q70,

        /// <summary>
        /// (official large) standard
        /// </summary>
        [Description("c/600x1200_90/")]
        [ImageFrame.Size(600, 1200)]
        C600X1200Q90,

        /// <summary>
        /// standard square
        /// </summary>
        [Description("c/100x100/")]
        [ImageFrame.Size(100, 100)]
        C100X100,

        /// <summary>
        /// standard square
        /// </summary>
        [Description("c/128x128/")]
        [ImageFrame.Size(128, 128)]
        C128X128,

        /// <summary>
        /// standard square
        /// </summary>
        [Description("c/150x150/")]
        [ImageFrame.Size(150, 150)]
        C150X150,

        /// <summary>
        /// standard square
        /// </summary>
        [Description("c/240x240/")]
        [ImageFrame.Size(240, 240)]
        C240X240,

        /// <summary>
        /// standard
        /// </summary>
        [Description("c/240x480/")]
        [ImageFrame.Size(240, 480)]
        C240X480,

        /// <summary>
        /// square
        /// </summary>
        [Description("c/250x250_80_a2/")]
        [ImageFrame.Size(250, 250)]
        C250X250Q80A2,

        /// <summary>
        /// standard square
        /// </summary>
        [Description("c/260x260_80/")]
        [ImageFrame.Size(260, 260)]
        C260X260Q80,

        /// <summary>
        /// square
        /// </summary>
        [Description("c/288x288_80_a2/")]
        [ImageFrame.Size(288, 288)]
        C288X288Q80A2,

        /// <summary>
        /// standard
        /// </summary>
        [Description("c/400x250_80/")]
        [ImageFrame.Size(400, 250)]
        C400X250Q80,

        /// <summary>
        /// standard
        /// </summary>
        [Description("c/600x600/")]
        [ImageFrame.Size(600, 600)]
        C600X600,

        /// <summary>
        /// standard
        /// </summary>
        [Description("c/768x1200_80/")]
        [ImageFrame.Size(768, 1200)]
        C768X1200Q80,

        /// <summary>
        /// standard square (1200x1200)
        /// </summary>
        [Description("")]
        [ImageFrame.Size(1200, 1200)]
        C

        /*
        /// <summary>
        /// crop
        /// </summary>
        [Description("c/300x200_a2/")]
        [ImageFrame.Size(300, 200)]
        C300X200A2,

        /// <summary>
        /// standard square webp
        /// </summary>
        [Description("c/360x360_10_webp/")]
        [ImageFrame.Size(360, 360)]
        C360X360Q10Webp,

        /// <summary>
        /// standard square webp
        /// </summary>
        [Description("c/540x540_10_webp/")]
        [ImageFrame.Size(540, 540)]
        C540X540Q10Webp,

        /// <summary>
        /// standard webp (square gives 600x600)
        /// </summary>
        [Description("c/600x1200_90_webp/")]
        [ImageFrame.Size(600, 600)]
        C600X1200Q90Webp,

        /// <summary>
        /// crop webp
        /// </summary>
        [Description("c/1080x600_10_a2_u1_webp/")]
        [ImageFrame.Size(1080, 600)]
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

    [JsonIgnore]
    public string SquareMediumUrl => ImageUrls.SquareMedium;

    [JsonIgnore]
    public string MediumUrl => ImageUrls.Medium;

    [JsonIgnore]
    public string LargeUrl => ImageUrls.Large;

    /// <inheritdoc cref="MangaImageUrls.Original"/>
    [JsonIgnore]
    public string OriginalUrl => ImageUrls.Original;
}
