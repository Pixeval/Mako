// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using Misaki;

namespace Mako.Model;

public interface IIdEntry : IIdentityInfo
{
    string IIdentityInfo.Id => Id is 0 ? "" : Id.ToString();

    string IPlatformInfo.Platform => Pixiv;

    new long Id { get; }
}

public interface IWorkEntry : IArtworkInfo, IIdEntry, ISerializable
{
    bool IsPrivate { get; }

    bool IsMuted { get; }

    new IReadOnlyList<Tag> Tags { get; }

    UserEntity User { get; }

    ImageUrls ThumbnailUrls { get; }

    AiType AiType { get; }

    XRestrict XRestrict { get; }
}
