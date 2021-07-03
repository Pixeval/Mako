using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mako.Engines
{
    /// <summary>
    ///     An highly abstracted fetch engine that fetches pages and yields results asynchronously
    ///     <para>
    ///         Just like a fetch engine, it continuously fetches pages, and each page may contains multiple
    ///         result entries, or an error response, at each iteration, it fetches one page and tries to
    ///         deserialize its content into a list of result entries, if an error response is occur, the
    ///         fetch engine stops and reports the iteration is over
    ///     </para>
    /// </summary>
    /// <typeparam name="E">The type of the results of the <see cref="IFetchEngine{E}" /></typeparam>
    [PublicAPI]
    public interface IFetchEngine<out E> : IAsyncEnumerable<E>, IMakoClientSupport, IEngineHandleSource
    {
        /// <summary>
        ///     How many pages have been fetches
        /// </summary>
        int RequestedPages { get; set; }
    }
}