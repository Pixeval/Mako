// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Mako.Utilities;
using Misaki;

namespace Mako.Model;

[Factory]
public partial record Novel : WorkBase, IWorkEntry, IArtworkInfo
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

    Uri IArtworkInfo.WebsiteUri => new($"https://www.pixiv.net/novel/show.php?id={Id}");

    DateTimeOffset IArtworkInfo.UpdateDate => CreateDate;

    DateTimeOffset IArtworkInfo.ModifyDate => CreateDate;

    IPreloadableEnumerable<IUser> IArtworkInfo.Authors => [User];

    IPreloadableEnumerable<IUser> IArtworkInfo.Uploaders => [];

    SafeRating IArtworkInfo.SafeRating => XRestrict switch
    {
        XRestrict.R18 => SafeRating.Explicit,
        XRestrict.R18G => SafeRating.Guro,
        XRestrict.Ordinary => SafeRating.General,
        _ => SafeRating.NotSpecified
    };

    ILookup<ITagCategory, ITag> IArtworkInfo.Tags => Tags.ToLookup(_ => ITagCategory.Empty, ITag (t) => t);

    [field: AllowNull, MaybeNull]
    IReadOnlyCollection<IImageFrame> IArtworkInfo.Thumbnails => field ??=
    [
        new ImageFrame(128, 128) { ImageUri = new(ThumbnailUrls.SquareMedium) },
        new ImageFrame(176, 352) { ImageUri = new(ThumbnailUrls.Medium) },
        new ImageFrame(240, 480) { ImageUri = new(ThumbnailUrls.Large) }
    ];

    public IReadOnlyDictionary<string, object> AdditionalInfo => new Dictionary<string, object>();

    public ImageType ImageType => ImageType.Other;

    public bool IsAiGenerated => AiType is AiType.AiGenerated;
}
