// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Net.Http;
using Mako.Global;

namespace Mako.Net;

public abstract class MakoClientSupportedHttpMessageHandler : HttpMessageHandler, IMakoClientSupport
{
    internal MakoClientSupportedHttpMessageHandler(
        MakoClient makoClient,
        MakoHttpMessageInvokerProvider invokerProvider)
    {
        MakoClient = makoClient;
        InvokerProvider = invokerProvider;
    }

    public MakoClient MakoClient { get; }

    private protected MakoHttpMessageInvokerProvider InvokerProvider { get; }
}
