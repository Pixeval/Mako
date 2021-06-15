using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Mako.Net
{
    internal class PixivApiHttpMessageHandler : MakoClientSupportedHttpMessageHandler
    {
        public PixivApiHttpMessageHandler(MakoClient makoClient)
        {
            MakoClient = makoClient;
        }
        
        public sealed override MakoClient MakoClient { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            MakoHttpOptions.UseHttpScheme(request);
            var headers = request.Headers;
            var host = request.RequestUri!.Host; // the 'RequestUri' is guaranteed to be nonnull here, because the 'HttpClient' will set it to 'BaseAddress' if its null

            headers.TryAddWithoutValidation("Accept-Language", MakoClient.ClientCulture.Name);

            var session = MakoClient.Session;

            switch (host)
            {
                case MakoHttpOptions.WebApiHost:
                    headers.TryAddWithoutValidation("Cookie", session.Cookie);
                    break;
                case MakoHttpOptions.AppApiHost:
                    headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
                    break;
            }

            INameResolver resolver = MakoHttpOptions.BypassRequiredHost.IsMatch(host) && session.Bypass
                ? MakoClient.Resolve<PixivApiNameResolver>()
                : MakoClient.Resolve<LocalMachineNameResolver>();
            return await MakoHttpOptions.CreateHttpMessageInvoker(resolver)
                .SendAsync(request, cancellationToken);
        }
    }
}