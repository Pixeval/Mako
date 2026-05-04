// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Utilities;

namespace Mako.Engine;

[method: MakoExtensionConstructor]
internal class ComputedEngine<T>(MakoClient makoClient, IAsyncEnumerable<T> result)
    : IFetchEngine<T>
{
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return result.GetAsyncEnumerator(cancellationToken);
    }

    public MakoClient MakoClient { get; } = makoClient;

    public EngineHandle EngineHandle { get; } = new EngineHandle(makoClient.CancelInstance);

    /// <summary>
    /// The <see cref="RequestedPages"/> in <see cref="ComputedEngine{T}"/> should always returns -1
    /// </summary>
    public int RequestedPages { get; set; } = -1;
}
