// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

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
    public required UserEntity UserInfo { get; set; }

    [JsonPropertyName("illusts")]
    public required IReadOnlyList<Illustration> Illustrations { get; set; } = [];

    [JsonPropertyName("novels")]
    public required IReadOnlyList<Novel> Novels { get; set; } = [];

    [JsonPropertyName("is_muted")]
    public required bool IsMuted { get; set; }
}

[DebuggerDisplay("{Id}: {Name}")]
[Factory]
public partial record UserEntity : IUser, IIdEntry
{
    [JsonPropertyName("id")]
    public required long Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; } = "";

    /// <summary>
    /// 只在<see cref="PixivSingleUserResponse"/>中才会有此项
    /// </summary>
    [JsonPropertyName("comment")]
    public string Description { get; set; } = "";

    public Uri WebsiteUri => new($"https://www.pixiv.net/users/{Id}");

    public Uri AppUri => new($"pixeval://user/{Id}");

    IReadOnlyCollection<IImageFrame> IUser.Avatar =>
    [
        new ImageFrame(170, 170) { ImageUri = new(ProfileImageUrls.Medium) }
    ];

    IReadOnlyDictionary<string, Uri> IUser.ContactInformation { get; } = new Dictionary<string, Uri>();

    IReadOnlyDictionary<string, object> IUser.AdditionalInfo { get; } = new Dictionary<string, object>();

    [JsonPropertyName("account")]
    public required string Account { get; set; } = "";

    [JsonPropertyName("profile_image_urls")]
    public required ProfileImageUrls ProfileImageUrls { get; set; }

    [JsonPropertyName("is_followed")]
    public bool IsFollowed { get; set; }
}
