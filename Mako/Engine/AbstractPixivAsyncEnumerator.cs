// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Mako.Net;
using Misaki;

namespace Mako.Engine;

/// <summary>
/// An abstract enumerator that encapsulates the required properties for Pixiv, it is intended to be
/// cooperated with the fetch engine, the fetch engine will fetch a new page(the page can be in multiple formats, such
/// as json),
/// and normally, a page can contain multiple result entries. When <see cref="MoveNextAsync" /> method is called, the
/// fetch
/// engine will try to get the next result entry in the current page, if there are no more entries, it will try to
/// fetch the next page, and if that also fails, then all the pages have been fetched, iteration is over.
/// </summary>
/// <typeparam name="TEntity">The entity that will be yield by the enumerator</typeparam>
/// <typeparam name="TRawEntity">The entity class corresponding to the result entry</typeparam>
/// <typeparam name="TFetchEngine">The fetch engine</typeparam>
public abstract class AbstractPixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine>(
    TFetchEngine pixivFetchEngine,
    MakoApiKind apiKind)
    : IAsyncEnumerator<TEntity>
    where TFetchEngine : class, IFetchEngine<TEntity>
    where TEntity : class, IMisakiModel
    where TRawEntity : class
{
    protected TFetchEngine PixivFetchEngine { get; } = pixivFetchEngine;

    protected MakoClient MakoClient => PixivFetchEngine.MakoClient;

    /// <summary>
    /// The result entries of the current page
    /// </summary>
    protected IEnumerator<TEntity>? CurrentEntityEnumerator;

    /// <summary>
    /// Indicates which kind of API this enumerator will use
    /// </summary>
    protected MakoApiKind ApiKind { get; } = apiKind;

    protected HttpClient ApiClient => MakoClient.GetMakoHttpClient(ApiKind);

    /// <summary>
    /// Indicates if the current operation has been cancelled
    /// </summary>
    protected bool IsCancellationRequested => PixivFetchEngine.EngineHandle.IsCancelled;

    /// <summary>
    /// The current result entry of <see cref="CurrentEntityEnumerator" />
    /// </summary>
    public TEntity Current => CurrentEntityEnumerator?.Current!;

    /// <summary>
    /// Moves the <see cref="MoveNextAsync" /> one step ahead, if fails, it will try to
    /// fetch a new page
    /// </summary>
    /// <returns></returns>
    public abstract ValueTask<bool> MoveNextAsync();

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return default;
    }
}
