using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;

namespace Mako.Net.Response
{
    internal class PixivSpotlightResponse
    {
        [JsonPropertyName("spotlight_articles")]
        public IEnumerable<SpotlightArticle>? SpotlightArticles { get; set; }

        [JsonPropertyName("next_url")]
        public string? NextUrl { get; set; }
    }
}