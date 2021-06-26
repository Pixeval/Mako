using System.Text.Json.Serialization;
using Mako.Model;

namespace Mako.Net.Response
{
    internal class PixivSingleIllustResponse
    {
        [JsonPropertyName("illust")]
        public IllustrationEssential.Illust Illust { get; set; }
    }
}