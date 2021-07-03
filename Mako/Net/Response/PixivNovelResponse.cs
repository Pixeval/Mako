using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;

namespace Mako.Net.Response
{
    internal class PixivNovelResponse
    {
        [JsonPropertyName("novels")]
        public IEnumerable<Novel>? Novels { get; set; }

        [JsonPropertyName("next_url")]
        public string? NextUrl { get; set; }
    }
}