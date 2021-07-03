using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mako.Model
{
    public record User
    {
        [JsonPropertyName("user")]
        public Info? UserInfo { get; set; }

        [JsonPropertyName("illusts")]
        public IEnumerable<Illustration>? Illusts { get; set; }

        [JsonPropertyName("is_muted")]
        public bool IsMuted { get; set; }

        public class Info
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("account")]
            public string? Account { get; set; }

            [JsonPropertyName("profile_image_urls")]
            public ProfileImageUrls? ProfileImageUrls { get; set; }

            [JsonPropertyName("comment")]
            public string? Comment { get; set; }

            [JsonPropertyName("is_followed")]
            public bool IsFollowed { get; set; }
        }
    }
}