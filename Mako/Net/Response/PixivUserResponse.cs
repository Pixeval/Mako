using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;

namespace Mako.Net.Response
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    internal class PixivUserResponse
    {
        [JsonPropertyName("user_previews")]
        public IEnumerable<UserEssential.User>? Users { get; set; }
        
        [JsonPropertyName("next_url")]
        public string? NextUrl { get; set; }
    }
}