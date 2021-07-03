using System;
using System.Collections.Generic;
using System.Threading;

namespace Mako.Engines
{
    /// <summary>
    ///     A <see cref="IFetchEngine{E}" /> that specialized for Pixiv, it holds an <see cref="MakoClient" />
    ///     and a <see cref="EngineHandle" /> to manage its lifetime
    /// </summary>
    /// <typeparam name="E">
    ///     <inheritdoc cref="IFetchEngine{E}" />
    /// </typeparam>
    public abstract class AbstractPixivFetchEngine<E> : IFetchEngine<E>
    {
        protected AbstractPixivFetchEngine(MakoClient makoClient, EngineHandle? engineHandle)
        {
            MakoClient = makoClient;
            EngineHandle = engineHandle ?? new EngineHandle(Guid.NewGuid());
        }

        public abstract IAsyncEnumerator<E> GetAsyncEnumerator(CancellationToken cancellationToken = new()); // the 'CancellationToken' is no longer useful, we use 'EngineHandle' to track the lifetime

        /// <summary>
        ///     The <see cref="MakoClient" /> that owns this <see cref="IFetchEngine{E}" />, it
        ///     shares its context such as <see cref="Mako.MakoClient.Configuration" /> with current
        ///     <see cref="IFetchEngine{E}" /> to provides the required fields when the <see cref="IFetchEngine{E}" />
        ///     performing its task
        /// </summary>
        public MakoClient MakoClient { get; }

        /// <summary>
        ///     How many pages have been fetched
        /// </summary>
        public int RequestedPages { get; set; }

        /// <summary>
        ///     The <see cref="EngineHandle" /> used to manage the lifetime of <see cref="IFetchEngine{E}" />
        /// </summary>
        public EngineHandle EngineHandle { get; }
    }
}