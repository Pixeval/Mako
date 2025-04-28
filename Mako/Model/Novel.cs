// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mako.Utilities;
using Misaki;

namespace Mako.Model;

[Factory]
public partial record Novel : WorkBase, IWorkEntry
{
    [JsonPropertyName("is_original")]
    public required bool IsOriginal { get; set; }

    [JsonPropertyName("page_count")]
    public required int PageCount { get; set; }

    [JsonPropertyName("text_length")]
    public required int TextLength { get; set; }

    [JsonPropertyName("is_mypixiv_only")]
    public required bool IsMypixivOnly { get; set; }

    [JsonPropertyName("is_x_restricted")]
    public required bool IsXRestricted { get; set; }

    [JsonPropertyName("total_comments")]
    public required int TotalComments { get; set; }

    [JsonPropertyName("novel_ai_type")]
    public required AiType AiType { get; set; }

    [field: AllowNull, MaybeNull]
    [JsonIgnore]
    public Uri WebsiteUri => field ??= new($"https://www.pixiv.net/novel/show.php?id={Id}");

    [field: AllowNull, MaybeNull]
    [JsonIgnore]
    public Uri AppUri => field ??= new($"pixeval://novel/{Id}");

    [JsonIgnore]
    DateTimeOffset IArtworkInfo.UpdateDate => CreateDate;

    [JsonIgnore]
    DateTimeOffset IArtworkInfo.ModifyDate => CreateDate;

    [JsonIgnore]
    IPreloadableList<IUser> IArtworkInfo.Authors => [User];

    [JsonIgnore]
    IPreloadableList<IUser> IArtworkInfo.Uploaders => [];

    [JsonIgnore]
    SafeRating IArtworkInfo.SafeRating => XRestrict switch
    {
        XRestrict.R18 => SafeRating.Explicit,
        XRestrict.R18G => SafeRating.Guro,
        XRestrict.Ordinary => SafeRating.General,
        _ => SafeRating.NotSpecified
    };

    [JsonIgnore]
    ILookup<ITagCategory, ITag> IArtworkInfo.Tags => Tags.ToLookup(_ => ITagCategory.Empty, ITag (t) => t);

    [field: AllowNull, MaybeNull]
    [JsonIgnore]
    IReadOnlyCollection<IImageFrame> IArtworkInfo.Thumbnails => field ??=
    [
        new ImageFrame(new ImageSize(128 / 2, 128)) { ImageUri = new(ThumbnailUrls.SquareMedium) },
        new ImageFrame(new ImageSize(352 / 2, 352)) { ImageUri = new(ThumbnailUrls.Medium) },
        new ImageFrame(new ImageSize(480 / 2, 480)) { ImageUri = new(ThumbnailUrls.Large) },
        new ImageFrame(new ImageSize(1200 / 2, 1200)) { ImageUri = new(ThumbnailUrls.NotCropped) }
    ];

    [JsonIgnore]
    public IReadOnlyDictionary<string, object> AdditionalInfo => new Dictionary<string, object>();

    [JsonIgnore]
    public ImageType ImageType => ImageType.Other;

    [JsonIgnore]
    public bool IsAiGenerated => AiType is AiType.AiGenerated;

    [JsonIgnore]
    public int Width => 0;

    [JsonIgnore]
    public int Height => 0;

    public string Serialize() => JsonSerializer.Serialize(this, typeof(Novel), AppJsonSerializerContext.Default);

    public static ISerializable Deserialize(string data) => (Novel) JsonSerializer.Deserialize(data, typeof(Novel), AppJsonSerializerContext.Default)!;

    [JsonIgnore]
    public string SerializeKey => typeof(Novel).FullName!;
}
