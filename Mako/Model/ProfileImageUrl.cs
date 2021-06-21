using System.Text.Json.Serialization;

namespace Mako.Model
{
    public record ProfileImageUrls
    {
        [JsonPropertyName("medium")]
        public string? Medium { get; set; }
    }
}