// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using Misaki;

namespace Mako.Model;

public interface IWorkEntry : IIdentityInfo
{
    string Title { get; }

    string Description { get; }

    int TotalView { get; }

    int TotalFavorite { get; }

    bool IsFavorite { get; set; }

    bool IsPrivate { get; set; }

    bool IsMuted { get; set; }

    Tag[] Tags { get; }

    UserInfo User { get; }

    DateTimeOffset CreateDate { get; }

    ImageUrls ThumbnailUrls { get; }

    AiType AiType { get; }

    XRestrict XRestrict { get; }
}
