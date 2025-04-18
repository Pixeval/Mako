using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record UgoiraMetadata
{
    [JsonPropertyName("zip_urls")]
    public required ZipUrls ZipUrls { get; set; }

    [JsonPropertyName("frames")]
    public required IReadOnlyList<Frame> Frames { get; set; } = [];

    [field: AllowNull, MaybeNull]
    public IReadOnlyList<int> Delays => field ??= [..Frames.Select(t => (int) t.Delay)];

    public string MediumUrl => ZipUrls.Medium;

    public string LargeUrl => ZipUrls.Large;
}
