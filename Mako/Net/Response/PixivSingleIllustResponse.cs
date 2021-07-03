using System.Text.Json.Serialization;
using Mako.Model;

namespace Mako.Net.Response
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    internal class PixivSingleIllustResponse
    {
        [JsonPropertyName("illust")]
        public Illustration? Illust { get; set; }
    }
}