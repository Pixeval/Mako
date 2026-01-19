// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Net.Http;
using Mako.Global;

namespace Mako.Net;

public abstract class MakoClientSupportedHttpMessageHandler(MakoClient makoClient) : HttpMessageHandler, IMakoClientSupport
{
    public MakoClient MakoClient { get; set; } = makoClient;

    public HttpMessageInvoker GetHttpMessageInvoker(bool domainFronting)
    {
        // TODO: HttpMessageInvoker是否应该复用？
        return domainFronting ? MakoHttpOptions.CreateHttpMessageInvoker() : MakoHttpOptions.CreateDirectHttpMessageInvoker(MakoClient);
    }
}
