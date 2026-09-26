// Copyright (c) Pixeval.
// Licensed under the GPL-3.0 License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Mako.Engine;
using Mako.Global.Exception;
using WebApiClientCore.Exceptions;

namespace Mako.Utilities;

public static class FetchEngineRetryHelper
{
    private static readonly TimeSpan[] _RetryDelays =
    [
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromMinutes(1)
    ];

    public static IAsyncEnumerable<TResult> StreamAsync<TResult>(
        IFetchEngine<TResult> engine,
        Func<int, TimeSpan>? retryDelayProvider = null,
        bool cancelEngineOnCancellation = true,
        CancellationToken token = default) =>
        StreamCoreAsync(engine, retryDelayProvider ?? GetRetryDelay, cancelEngineOnCancellation, token);

    public static async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        Func<int, TimeSpan>? retryDelayProvider = null,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        retryDelayProvider ??= GetRetryDelay;
        var retryCount = 0;
        while (true)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                return await operation(token).ConfigureAwait(false);
            }
            catch (Exception exception) when (!token.IsCancellationRequested && IsRetryable(exception))
            {
                var delay = retryDelayProvider(retryCount++);
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, token).ConfigureAwait(false);
            }
        }
    }

    private static async IAsyncEnumerable<TResult> StreamCoreAsync<TResult>(
        IFetchEngine<TResult> engine,
        Func<int, TimeSpan> retryDelayProvider,
        bool cancelEngineOnCancellation,
        [EnumeratorCancellation] CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(engine);
        ArgumentNullException.ThrowIfNull(retryDelayProvider);

        using var registration = cancelEngineOnCancellation
            ? token.Register(engine.EngineHandle.Cancel)
            : default;
        await using var enumerator = engine.GetAsyncEnumerator(token);
        var retryCount = 0;
        while (true)
        {
            token.ThrowIfCancellationRequested();
            if (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                retryCount = 0;
                yield return enumerator.Current;
                continue;
            }

            if (engine.EngineHandle.IsCompleted)
                yield break;
            if (engine.EngineHandle.IsCancelled)
                throw new OperationCanceledException("The fetch engine was cancelled.", token);
            if (engine.EngineHandle.LastException is { } exception && !IsRetryable(exception))
                ExceptionDispatchInfo.Capture(exception).Throw();

            var delay = retryDelayProvider(retryCount++);
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, token).ConfigureAwait(false);
        }
    }

    private static TimeSpan GetRetryDelay(int retryCount) =>
        _RetryDelays[Math.Min(retryCount, _RetryDelays.Length - 1)];

    private static bool IsRetryable(Exception exception) => exception switch
    {
        MakoNetworkException { StatusCode: var status } => status is -1 || IsRetryableStatus(status),
        ApiResponseStatusException status => IsRetryableStatus((int) status.StatusCode),
        HttpRequestException { StatusCode: null } => true,
        HttpRequestException { StatusCode: { } status } => IsRetryableStatus((int) status),
        OperationCanceledException { InnerException: TimeoutException } => true,
        _ => false
    };

    private static bool IsRetryableStatus(int status) => status is 408 or 425 or 429 or >= 500 and <= 599;
}
