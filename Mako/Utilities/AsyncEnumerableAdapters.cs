// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mako.Utilities;

public class AdaptedAsyncEnumerator<T>(IEnumerator<T> outerEnumerator, CancellationToken cancellationToken = default)
    : IAsyncEnumerator<T>
{
    public ValueTask DisposeAsync()
    {
        outerEnumerator.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return ValueTask.FromResult(!cancellationToken.IsCancellationRequested && outerEnumerator.MoveNext());
    }

    public T Current => outerEnumerator.Current;
}
