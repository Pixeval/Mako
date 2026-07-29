// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Global.Enum;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record BookmarkDetail
{
    [JsonPropertyName("is_bookmarked")]
    public required bool IsBookmarked { get; set; }

    [JsonPropertyName("restrict")]
    public PrivacyPolicy Restrict { get; set; }

    [JsonPropertyName("tags")]
    public required IReadOnlyList<BookmarkDetailTag> Tags { get; set; } = [];
}

[Factory]
public partial record BookmarkDetailTag
{
    [JsonPropertyName("is_registered")]
    public required bool IsRegistered { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
