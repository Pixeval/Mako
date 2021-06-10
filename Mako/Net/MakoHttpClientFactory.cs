using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Mako.Util;

namespace Mako.Net
{
    internal class MakoHttpClientFactory
    {
        private readonly IReadOnlyDictionary<MakoApiKind, MakoTaggedHttpClient> clients;

        public MakoHttpClientFactory(IEnumerable<MakoTaggedHttpClient> clients)
        {
            this.clients = clients.ToDictionary(c => c.ClientKind, Functions.Identity<MakoTaggedHttpClient>());
        }

        public MakoTaggedHttpClient this[MakoApiKind kind] => clients[kind];

        public static MakoTaggedHttpClient Create(
            MakoApiKind kind,
            HttpMessageHandler handler,
            Action<MakoTaggedHttpClient>? action = null)
        {
            var mako = new MakoTaggedHttpClient(handler) {ClientKind = kind};
            action?.Let(ac => ac!(mako));
            return mako;
        }
    }
}