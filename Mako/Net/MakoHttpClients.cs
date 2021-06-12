using System;
using System.Net.Http;
using System.Threading;
using Autofac;
using Mako.Util;

namespace Mako.Net
{
    internal static class MakoHttpClients
    {
        public static MakoTaggedHttpClient GetMakoTaggedHttpClient(this MakoClient makoClient, MakoApiKind kind)
        {
            return makoClient.MakoServices.Resolve<MakoHttpClientFactory>()![kind];
        }
    }
}