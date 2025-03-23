// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record ProfileImageUrls
{
    [JsonPropertyName("medium")]
    public required string Medium { get; set; } = DefaultImageUrls.ImageNotAvailable;
}
