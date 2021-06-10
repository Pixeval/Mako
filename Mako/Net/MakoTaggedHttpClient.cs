using System.Net.Http;
using JetBrains.Annotations;

namespace Mako.Net
{
    internal class MakoTaggedHttpClient : HttpClient
    {
        public MakoApiKind ClientKind { get; init; }

        public MakoTaggedHttpClient()
        {
        }

        public MakoTaggedHttpClient([NotNull] HttpMessageHandler handler) : base(handler)
        {
        }

        public MakoTaggedHttpClient([NotNull] HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
        }
    }
}