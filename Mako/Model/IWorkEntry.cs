// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Collections.Generic;
using Misaki;

namespace Mako.Model;

public interface IIdEntry : IIdentityInfo
{
    string IIdentityInfo.Id => Id.ToString();

    string IIdentityInfo.Platform => Pixiv;

    new long Id { get; }
}

public interface IWorkEntry : IArtworkInfo, IIdEntry
{
    bool IsPrivate { get; }

    bool IsMuted { get; }

    new IReadOnlyList<Tag> Tags { get; }

    UserEntity User { get; }

    ImageUrls ThumbnailUrls { get; }

    AiType AiType { get; }

    XRestrict XRestrict { get; }
}
