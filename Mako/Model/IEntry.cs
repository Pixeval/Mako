// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using Misaki;

namespace Mako.Model;

public interface IWorkEntry : IIdentityInfo
{
    int TotalView { get; }

    int TotalFavorite { get; }

    bool IsBookmarked { get; set; }

    bool IsPrivate { get; set; }

    bool IsMuted { get; set; }

    Tag[] Tags { get; }

    string Title { get; }

    string Description { get; }

    UserInfo User { get; }

    DateTimeOffset CreateDate { get; }

    ImageUrls ThumbnailUrls { get; }

    AiType AiType { get; }

    XRestrict XRestrict { get; }
}
