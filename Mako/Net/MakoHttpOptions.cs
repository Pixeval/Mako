// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public static Dictionary<string, IPAddress[]> NameResolvers { get; } = new()
    {
        [ImageHost] = [],
        [WebApiHost] = [],
        [AccountHost] = [],
        [AppApiHost] = [],
        [ImageHost2] = [],
        [OAuthHost] = []
    };

    public static void SetNameResolver(string host, string[] nameResolvers)
    {
        NameResolvers[host] = [.. nameResolvers.Select(IPAddress.Parse)];
    }

    public static readonly Regex DomainFrontingRequiredHost = MyRegex();

    [GeneratedRegex(@"^app-api\.pixiv\.net$|^www\.pixiv\.net$")]
    private static partial Regex MyRegex();

    public static void UseHttpScheme(HttpRequestMessage request)
    {
        if (request.RequestUri is not null)
        {
            request.RequestUri = new UriBuilder(request.RequestUri)
            {
                Scheme = "http"
            }.Uri;
        }
    }

    public static HttpMessageInvoker CreateHttpMessageInvoker()
    {
        return new HttpMessageInvoker(new SocketsHttpHandler
        {
            ConnectCallback = DomainFrontingConnectCallback
        });
    }

    public static HttpMessageInvoker CreateDirectHttpMessageInvoker(MakoClient makoClient)
    {
        var useProxy = makoClient.CurrentSystemProxy is not null;
        return new HttpMessageInvoker(new SocketsHttpHandler
        {
            UseProxy = useProxy,
            Proxy = makoClient.CurrentSystemProxy
        });
    }

    private static async ValueTask<Stream> DomainFrontingConnectCallback(SocketsHttpConnectionContext context, CancellationToken token)
        => await CreateConnectionAsync(context.InitialRequestMessage.RequestUri!.Host, token).ConfigureAwait(false);

    private static async Task<IPAddress[]> GetAddressesAsync(string host, CancellationToken token = default) 
        => NameResolvers.TryGetValue(host, out var ips) 
            ? ips 
            : await Dns.GetHostAddressesAsync(host, token).ConfigureAwait(false);

    public static async Task<SslStream> CreateConnectionAsync(string host, CancellationToken token = default)
    {
        var client = new TcpClient(); // disposed by netStream
        var ipAddresses = await GetAddressesAsync(host, token).ConfigureAwait(false);
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
}
