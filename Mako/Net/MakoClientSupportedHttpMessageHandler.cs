using System.Net.Http;

namespace Mako.Net
{
    public abstract class MakoClientSupportedHttpMessageHandler : HttpMessageHandler, IMakoClientSupport
    {
        public abstract MakoClient MakoClient { get; set; }
    }
}