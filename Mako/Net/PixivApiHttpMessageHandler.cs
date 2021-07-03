﻿#region Copyright (c) Pixeval/Mako

// MIT License
// 
// Copyright (c) Pixeval 2021 Mako/PixivApiHttpMessageHandler.cs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

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

            headers.TryAddWithoutValidation("Accept-Language", MakoClient.Configuration.CultureInfo.Name);

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

            INameResolver resolver = MakoHttpOptions.BypassRequiredHost.IsMatch(host) && MakoClient.Configuration.Bypass
                ? MakoClient.Resolve<PixivApiNameResolver>()
                : MakoClient.Resolve<LocalMachineNameResolver>();
            return await MakoHttpOptions.CreateHttpMessageInvoker(resolver) // TODO use a unique http message invoker through the application lifetime
                .SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}