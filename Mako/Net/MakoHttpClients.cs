namespace Mako.Net
{
    internal static class MakoHttpClients
    {
        public static MakoTaggedHttpClient GetMakoTaggedHttpClient(this MakoClient makoClient, MakoApiKind kind)
        {
            return makoClient.GetService<MakoHttpClientFactory>()![kind];
        }
    }
}