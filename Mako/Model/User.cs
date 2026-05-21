// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Mako.Net.Response;
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

[DebuggerDisplay("{Id}: {Name}")]
[Factory]
public partial record UserInfo : CommentUser
{
    [JsonPropertyName("comment")]
    public override string Description { get; set; } = "";

    /// <remarks>
    /// <see cref="SingleUserResponse"/> 等结构里可能没有此项
    /// </remarks>
    [JsonPropertyName("is_accept_request")]
    public bool IsAcceptRequest { get; set; }

    [JsonPropertyName("is_followed")]
    public bool IsFollowed { get; set; }
}

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

    public abstract string Description { get; set; }

    IReadOnlyCollection<IImageFrame> IUser.Avatar =>
    [
        new ImageFrame(new ImageSize(170, 170)) { ImageUri = new(AvatarUrl) }
    ];

    IReadOnlyDictionary<string, Uri> IUser.ContactInformation { get; } = new Dictionary<string, Uri>();

    IReadOnlyDictionary<string, object> IUser.AdditionalInfo { get; } = new Dictionary<string, object>();

    public Uri WebsiteUri => new($"https://www.pixiv.net/users/{Id}");

    public Uri AppUri => new($"pixeval://user/{Id}");
}
