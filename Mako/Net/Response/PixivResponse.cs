using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;

namespace Mako.Net.Response
{
    
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    internal class PixivResponse
    {
        [JsonPropertyName("illusts")]
        public List<IllustrationEssential.Illust>? Illusts { get; set; }

        [JsonPropertyName("next_url")]
        public string? NextUrl { get; set; }
    }
}