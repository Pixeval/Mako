// Copyright (c) Pixeval.Utilities.
// Licensed under the GPL v3 License.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mako.Utilities;

internal static class Functions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<T, T> Identity<T>()
    {
        return static t => t;
    }

    extension<TIn>(TIn obj)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ROut Let<ROut>(Func<TIn, ROut> block)
        {
            return block(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Let(Action<TIn> block)
        {
            block(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TIn Apply(Action<TIn> block)
        {
            block(obj);
            return obj;
        }
    }

    public static async Task<Result<TResult>> WithTimeoutAsync<TResult>(Task<TResult> task, int timeoutMills)
    {
        using var cancellationToken = new CancellationTokenSource();
        if (await Task.WhenAny(task, Task.Delay(timeoutMills, cancellationToken.Token)).ConfigureAwait(false) == task)
        {
            if (!cancellationToken.IsCancellationRequested)
                await cancellationToken.CancelAsync();
            return Result<TResult>.AsSuccess(task.Result);
        }

        return Result<TResult>.AsFailure();
    }

    public static async Task<Result<TResult>> RetryAsync<TResult>(Func<Task<TResult>> body, int attempts = 3, int timeoutMs = 0)
    {
        var counter = 0;
        Exception? cause = null;
        while (counter++ < attempts)
        {
            var task = body();
            try
            {
                if (await WithTimeoutAsync(task, timeoutMs).ConfigureAwait(false) is Result<TResult>.Success result)
                {
                    return result;
                }
            }
            catch (Exception e)
            {
                cause = e;
            }
        }

        return Result<TResult>.AsFailure(cause);
    }
}
