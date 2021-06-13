using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Mako.Util
{
    [PublicAPI]
    public static class Functions
    {
        public static Func<T, T> Identity<T>() => static t => t;

        public static ROut? Let<TIn, ROut>(this TIn obj, Func<TIn, ROut> block)
        {
            return obj is not null ? block(obj) : default;
        }
        
        public static void Let<TIn>(this TIn obj, Action<TIn?> block)
        {
            if (obj is not null)
            {
                block(obj);
            }
        }

        public static async Task<Result> WithTimeout<TResult>(Task<TResult> task, int timeoutMills)
        {
            using var cancellationToken = new CancellationTokenSource();
            if (await Task.WhenAny(task, Task.Delay(timeoutMills, cancellationToken.Token)) == task)
            {
                cancellationToken.Cancel();
                return Result.OfSuccess(task.Result);
            }

            return Result.OfFailure();
        }
        
        public static async Task<Result> RetryAsync<TResult>(Func<Task<TResult>> body, int attempts = 3, int timeoutMills = 0)
        {
            var counter = 0;
            Exception? cause = null;
            while (counter++ < attempts)
            {
                var task = body();
                using var cancellationToken = new CancellationTokenSource();
                try
                {
                    if (await WithTimeout(task, timeoutMills) is Result.Success<TResult> result)
                    {
                        return result;
                    }
                }
                catch (Exception e)
                {
                    cause = e;
                }
            }
            return Result.OfFailure(cause);
        }
    }
}