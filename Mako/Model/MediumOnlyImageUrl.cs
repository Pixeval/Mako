// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record MediumOnlyImageUrl
{
    [JsonPropertyName("medium")]
    public required string Medium { get; set; } = DefaultImageUrls.ImageNotAvailable;
}
