// Copyright (c) Mako.
// Licensed under the GPL v3 License.

using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record ImageUrls
{
    [JsonPropertyName("square_medium")]
    public required string SquareMedium { get; set; } = DefaultImageUrls.ImageNotAvailable;

    [JsonPropertyName("medium")]
    public required string Medium { get; set; } = DefaultImageUrls.ImageNotAvailable;

    [JsonPropertyName("large")]
    public required string Large { get; set; } = DefaultImageUrls.ImageNotAvailable;

    [JsonIgnore]
    public string NotCropped => Large.Replace("c/600x1200_90/", "").Replace("c/240x480_80/", "");
}
