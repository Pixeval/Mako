using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record ZipUrls
{
    [JsonPropertyName("medium")]
    public required string Medium { get; set; } = DefaultImageUrls.ImageNotAvailable;

    public string Large => Medium.Replace("600x600", "1920x1080");

    public string Original => Medium.Replace("600x600", "");
}
