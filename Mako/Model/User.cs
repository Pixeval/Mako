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
public partial record User : IIdEntry
{
    public long Id => UserInfo.Id;

    [JsonPropertyName("user")]
    public required UserInfo UserInfo { get; set; }

    [JsonPropertyName("illusts")]
    public required Illustration[] Illustrations { get; set; } = [];

    [JsonPropertyName("novels")]
    public required Novel[] Novels { get; set; } = [];

    [JsonPropertyName("is_muted")]
    public required bool IsMuted { get; set; }
}

[DebuggerDisplay("{Id}: {Name}")]
[Factory]
public partial record UserInfo : IUser
{
    [JsonPropertyName("id")]
    public required long Id { get; set; }

    public string Platform => IIdentityInfo.Pixiv;

    [JsonPropertyName("name")]
    public required string Name { get; set; } = "";

    public string Description { get; }
    public Uri Uri { get; }
    public IReadOnlyList<IImageFrame> Avatar { get; }
    public IReadOnlyDictionary<string, Uri> ContactInformation { get; }
    public IReadOnlyDictionary<string, object> AdditionalInfo { get; }

    [JsonPropertyName("account")]
    public required string Account { get; set; } = "";

    [JsonPropertyName("profile_image_urls")]
    public required ProfileImageUrls ProfileImageUrls { get; set; }

    [JsonPropertyName("is_followed")]
    public bool IsFollowed { get; set; }
}
