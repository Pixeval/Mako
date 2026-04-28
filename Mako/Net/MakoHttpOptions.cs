// Copyright (c) Mako.
// Licensed under the MIT License.

using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Mako.Net;

public static partial class MakoHttpOptions
{
    public const string AppApiBaseUrl = "https://app-api.pixiv.net";

    public const string WebApiBaseUrl = "https://www.pixiv.net";

    public const string OAuthBaseUrl = "https://oauth.secure.pixiv.net";

    public const string ImageHost = "i.pximg.net";

    public const string ImageHost2 = "s.pximg.net";

    public const string WebApiHost = "www.pixiv.net"; // experiments revealed that the secondary domain 'www' is required 

    public const string AppApiHost = "app-api.pixiv.net";

    public const string AccountHost = "accounts.pixiv.net";

    public const string OAuthHost = "oauth.secure.pixiv.net";

    [GeneratedRegex(@"^app-api\.pixiv\.net$|^www\.pixiv\.net$")]
    public static partial Regex DomainFrontingRequiredHost { get; }

    extension(MakoClient makoClient)
    {
        internal async ValueTask<Stream> DomainFrontingConnectCallback(SocketsHttpConnectionContext context, CancellationToken token)
            => await makoClient.CreateConnectionAsync(context.InitialRequestMessage.RequestUri!.Host, token).ConfigureAwait(false);

        private async Task<SslStream> CreateConnectionAsync(string host, CancellationToken token = default)
        {
            var client = new TcpClient(); // disposed by netStream
            var ipAddresses = await makoClient.GetAddressesAsync(host, token).ConfigureAwait(false);
            await client.ConnectAsync(ipAddresses, 443, token).ConfigureAwait(false);
            var netStream = client.GetStream(); // disposed by sslStream
            var sslStream = new SslStream(netStream, false, (_, _, _, _) => true);
            try
            {
                await sslStream.AuthenticateAsClientAsync("").ConfigureAwait(false);
                return sslStream;
            }
            catch
            {
                await sslStream.DisposeAsync().ConfigureAwait(false);
                throw;
            }
        }

        private async Task<IPAddress[]> GetAddressesAsync(string host, CancellationToken token = default)
            => makoClient.Configuration.NameResolvers.TryGetValue(host, out var ips) && ips.Length > 0
                ? ips
                : await Dns.GetHostAddressesAsync(host, token).ConfigureAwait(false);
    }
}
