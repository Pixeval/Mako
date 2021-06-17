using System;
using System.Collections.Generic;
using System.Threading;

namespace Mako.Engines
{
    
    /// <summary>
    /// 一个针对Pixiv所特化的<see cref="IFetchEngine{E}"/>，提供了可以追踪其生命周期的句柄
    /// </summary>
    /// <typeparam name="E"><inheritdoc cref="IFetchEngine{E}"/></typeparam>
    internal abstract class AbstractPixivFetchEngine<E> : IFetchEngine<E>
    {
        protected AbstractPixivFetchEngine(EngineHandle? engineHandle)
        {
            EngineHandle = engineHandle ?? new EngineHandle(Guid.NewGuid());
        }

        public abstract IAsyncEnumerator<E> GetAsyncEnumerator(CancellationToken cancellationToken = new()); // the 'CancellationToken' is no longer useful, we use 'EngineHandle' to track the lifetime

        public abstract MakoClient MakoClient { get; set; }
        
        public int RequestedPages { get; set; }

        public EngineHandle EngineHandle { get; }
    }
}