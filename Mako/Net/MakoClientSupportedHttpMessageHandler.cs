using System.Net.Http;
using JetBrains.Annotations;

namespace Mako.Net
{
    public abstract class MakoClientSupportedHttpMessageHandler : HttpMessageHandler, IMakoClientSupport
    {
        public abstract MakoClient MakoClient { get; set; }
    }
}