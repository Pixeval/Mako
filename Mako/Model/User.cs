// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Mako.Net.Responses;
using Mako.Utilities;
using Misaki;

namespace Mako.Model;

[DebuggerDisplay("{UserInfo}")]
[Factory]
public partial record User : IIdEntry
{
    public long Id => UserInfo.Id;

    [JsonPropertyName("user")]
    public required UserInfo UserInfo { get; set; }

    [JsonPropertyName("illusts")]
    public required IReadOnlyList<Illustration> Illustrations { get; set; } = [];

    [JsonPropertyName("novels")]
    public required IReadOnlyList<Novel> Novels { get; set; } = [];

    [JsonPropertyName("is_muted")]
    public required bool IsMuted { get; set; }
}

/// <summary>
/// <see cref="SingleUserResponse"/> 专用
/// </summary>
[Factory]
public partial record SingleUserInfo : BookmarkUserInfo
{
    [JsonPropertyName("comment")]
    public required string Description { get; set; } = "";
}


/// <summary>
/// <see cref="MakoClient.BookmarkUserWork"/> 专用
/// </summary>
[Factory]
public partial record BookmarkUserInfo : UserInfo
{
    [JsonPropertyName("is_access_blocking_user")]
    public required bool IsAccessBlockingUser { get; set; }
}

/// <summary>
/// 多个已关注属性，一般<see cref="WorkBase"/>使用
/// </summary>
[Factory]
public partial record UserInfo : AvatarUser
{
    /// <summary>
    /// 若用户将作品设为非公开可见或删除，则此属性可能不存在
    /// </summary>
    [JsonPropertyName("is_followed")]
    public bool IsFollowed { get; set; }

    /// <remarks>
    /// ["restricted_mode"]，在<see cref="WorkBase"/>应该不使用，只在<see cref="User"/>、<see cref="SingleUserInfo"/>中使用
    /// </remarks>
    [JsonPropertyName("restriction_attributes")]
    public IReadOnlyList<string>? RestrictionAttributes { get; set; }
}

/// <summary>
/// 除 <see cref="TokenUser"/> 的用户基类，多了用户头像的属性，也被 <see cref="Comment"/>、<see cref="SeriesDetailBase"/> 直接使用
/// </summary>
[Factory]
public partial record AvatarUser : UserBasicInfo
{
    /// <remarks>
    /// <see cref="SingleUserResponse"/> 等结构里可能没有此项
    /// </remarks>
    [JsonPropertyName("is_accept_request")]
    public bool IsAcceptRequest { get; set; }

    [JsonPropertyName("profile_image_urls")]
    public required MediumOnlyImageUrl ProfileImageUrls { get; set; }

    /// <inheritdoc />
    public override string AvatarUrl => ProfileImageUrls.Medium;
}

/// <summary>
/// 所有用户信息的基类，包含id、name、account、avatar等基本信息
/// </summary>
[DebuggerDisplay("{Id}: {Name}")]
public abstract record UserBasicInfo : IUser, IIdEntry
{
    /// <summary>
    /// 在<see cref="TokenUser"/>中是string，别的都是long
    /// </summary>
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required long Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; } = "";

    [JsonPropertyName("account")]
    public required string Account { get; set; } = "";

    /// <summary>
    /// 170x170
    /// </summary>
    public abstract string AvatarUrl { get; }

    string IUser.Description => "";

    IReadOnlyCollection<IImageFrame> IUser.Avatar =>
    [
        new ImageFrame(new ImageSize(170, 170)) { ImageUri = new(AvatarUrl) }
    ];

    IReadOnlyDictionary<string, Uri> IUser.ContactInformation { get; } = new Dictionary<string, Uri>();

    IReadOnlyDictionary<string, object> IUser.AdditionalInfo { get; } = new Dictionary<string, object>();

    public Uri WebsiteUri => new($"https://www.pixiv.net/users/{Id}");

    public Uri AppUri => new($"pixeval://user/{Id}");
}
