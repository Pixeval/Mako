using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Mako.Util
{
    /// <summary>
    /// 把一个同步的<see cref="IEnumerator{T}"/>适配到异步的<see cref="IAsyncEnumerator{T}"/>
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    [PublicAPI]
    public class AdaptedAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _outerEnumerator;
        private readonly CancellationToken _cancellationToken;

        public AdaptedAsyncEnumerator(IEnumerator<T> outerEnumerator, CancellationToken cancellationToken = new())
        {
            _outerEnumerator = outerEnumerator;
            _cancellationToken = cancellationToken;
        }

        public ValueTask DisposeAsync()
        {
            _outerEnumerator.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(!_cancellationToken.IsCancellationRequested && _outerEnumerator.MoveNext());
        }

        public T Current => _outerEnumerator.Current;
    }
}