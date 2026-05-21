// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record Comment : IIdEntry
{
    [JsonPropertyName("id")]
    public required long Id { get; set; }

    [JsonPropertyName("comment")]
    public required string Content { get; set; } = "";

    [JsonPropertyName("date")]
    public required DateTimeOffset Date { get; set; }

    [JsonPropertyName("user")]
    public required CommentUser User { get; set; }

    [JsonPropertyName("has_replies")]
    public required bool HasReplies { get; set; }

    [JsonPropertyName("stamp")]
    public required Stamp? Stamp { get; set; }
}

[Factory]
public partial record Stamp
{
    [JsonPropertyName("stamp_id")]
    public required long StampId { get; set; }

    [JsonPropertyName("stamp_url")]
    public required string StampUrl { get; set; } = DefaultImageUrls.ImageNotAvailable;
}

[Factory]
public partial record CommentUser : UserBasicInfo
{
    [JsonPropertyName("profile_image_urls")]
    public required MediumOnlyImageUrl ProfileImageUrls { get; set; }

    /// <inheritdoc />
    public override string AvatarUrl => ProfileImageUrls.Medium;

    /// <inheritdoc />
    public override string Description
    {
        get => "";
        set { }
    }
}
