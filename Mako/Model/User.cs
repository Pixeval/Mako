// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Mako.Utilities;
using Misaki;

namespace Mako.Model;

[DebuggerDisplay("{UserInfo}")]
[Factory]
public partial record User : IIdentityInfo
{
    public string Platform => IIdentityInfo.Pixiv;

    public long Identity => UserInfo.Identity;

    public string Id => Identity.ToString();

    [JsonPropertyName("user")]
    public required UserInfo UserInfo { get; set; }

    [JsonPropertyName("illusts")]
    public required Illustration[] Illustrations { get; set; } = [];

    [JsonPropertyName("novels")]
    public required Novel[] Novels { get; set; } = [];

    [JsonPropertyName("is_muted")]
    public required bool IsMuted { get; set; }
}

[DebuggerDisplay("{Identity}: {Name}")]
[Factory]
public partial record UserInfo : IUser
{
    public string Platform => IIdentityInfo.Pixiv;
    
    [JsonPropertyName("id")]
    public required long Identity { get; set; }

    public string Id => Identity.ToString();

    [JsonPropertyName("name")]
    public required string Name { get; set; } = "";

    public virtual string Description { get; set; } = "";

    Uri IUser.WebsiteUri => new($"https://www.pixiv.net/users/{Identity}");

    IReadOnlyList<IImageFrame> IUser.Avatar =>
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
