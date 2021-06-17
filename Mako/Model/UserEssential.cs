using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mako.Model
{
    public static class UserEssential
    {
        public class User
        {
            [JsonPropertyName("user")]
            public UserInfo UserInfo { get; set; }
            
            [JsonPropertyName("illusts")]
            public IEnumerable<IllustrationEssential.Illust> Illusts { get; set; }
            
            [JsonPropertyName("is_muted")]
            public bool IsMuted { get; set; }
        }
    
        public class UserInfo
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("account")]
            public string Account { get; set; }

            [JsonPropertyName("profile_image_urls")]
            public IllustrationEssential.ProfileImageUrls ProfileImageUrls { get; set; }

            [JsonPropertyName("is_followed")]
            public bool IsFollowed { get; set; }
        }
    }
}