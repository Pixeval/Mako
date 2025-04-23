using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record ZipUrls
{
    /// <summary>
    /// 600x600 width prior
    /// </summary>
    [JsonPropertyName("medium")]
    public required string Medium { get; set; } = DefaultImageUrls.ImageNotAvailable;

    /// <summary>
    /// 1920x1080 height prior
    /// </summary>
    public string Large => Medium.Replace("600x600", "1920x1080");
}
