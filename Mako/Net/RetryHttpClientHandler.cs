using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mako.Util;

namespace Mako.Net
{
    internal class RetryHttpClientHandler : HttpMessageHandler, IMakoClientSupport
    {
        private readonly HttpMessageInvoker _delegatedHandler;

        public RetryHttpClientHandler(HttpMessageHandler delegatedHandler)
        {
            _delegatedHandler = new HttpMessageInvoker(delegatedHandler);
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global ! Dependency Injected
        public MakoClient MakoClient { get; set; } = null!;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Functions.RetryAsync(() => _delegatedHandler.SendAsync(request, cancellationToken), 2, MakoClient!.Configuration.ConnectionTimeout).ConfigureAwait(false) switch
            {
                Result<HttpResponseMessage>.Success (var response) => response,
                Result<HttpResponseMessage>.Failure failure        => throw failure.Cause ?? new HttpRequestException(),
                _                                                  => throw new InvalidOperationException("Unexpected case")
            };
        }
    }
}