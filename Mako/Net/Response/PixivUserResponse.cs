using System.Text.Json.Serialization;
using Mako.Model;

namespace Mako.Net.Response
{
    internal class PixivUserResponse
    {
        [JsonPropertyName("user_previews")]
        public UserEssential.User Users { get; set; }
        
        [JsonPropertyName("next_url")]
        public string NextUrl { get; set; }
    }
}