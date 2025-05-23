// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Mako.Utilities;
using Misaki;

namespace Mako.Engine;

internal abstract class RecursivePixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine>(TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind)
    : AbstractPixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine>(pixivFetchEngine, makoApiKind)
    where TEntity : class, IMisakiModel
    where TRawEntity : class
    where TFetchEngine : class, IFetchEngine<TEntity>
{
    private TRawEntity? RawEntity { get; set; }

    protected abstract string? NextUrl(TRawEntity? rawEntity);

    protected abstract string InitialUrl { get; }

    protected abstract IEnumerator<TEntity>? GetNewEnumerator(TRawEntity? rawEntity);

    protected virtual bool HasNextPage()
    {
        return !string.IsNullOrEmpty(NextUrl(RawEntity));
    }

    public override async ValueTask<bool> MoveNextAsync()
    {
        if (IsCancellationRequested)
        {
            PixivFetchEngine.EngineHandle.Complete(); // Set the state of the 'PixivFetchEngine' to Completed
            return false;
        }

        if (RawEntity is null)
        {
            switch (await GetJsonResponseAsync(InitialUrl).ConfigureAwait(false))
            {
                case Result<TRawEntity>.Success(var raw):
                    Update(raw);
                    break;
                case Result<TRawEntity>.Failure(var exception):
                    if (exception is not null)
                    {
                        MakoClient.LogException(exception);
                    }

                    PixivFetchEngine.EngineHandle.Complete();
                    return false;
            }
        }

        if (CurrentEntityEnumerator!.MoveNext()) // If the enumerator can proceeds then return true
        {
            return true;
        }

        if (!HasNextPage()) // Check if there are more pages, return false if not
        {
            PixivFetchEngine.EngineHandle.Complete();
            return false;
        }

        if (await GetJsonResponseAsync(NextUrl(RawEntity)!).ConfigureAwait(false) is Result<TRawEntity>.Success(var value)) // Else request a new page
        {
            if (IsCancellationRequested)
            {
                PixivFetchEngine.EngineHandle.Complete();
                return false;
            }

            Update(value);
            _ = CurrentEntityEnumerator.MoveNext();
            return true;
        }

        PixivFetchEngine.EngineHandle.Complete();
        return false;
    }

    private void Update(TRawEntity rawEntity)
    {
        RawEntity = rawEntity;
        CurrentEntityEnumerator = GetNewEnumerator(rawEntity) ?? Enumerable.Empty<TEntity>().GetEnumerator();
        ++PixivFetchEngine.RequestedPages;
    }
}

internal static class RecursivePixivAsyncEnumerators
{
    public abstract class BaseRecursivePixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine>(TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind, string initialUrl)
        : RecursivePixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine>(pixivFetchEngine, makoApiKind)
        where TEntity : class, IMisakiModel
        where TRawEntity : class, IPixivNextUrlResponse<TEntity>
        where TFetchEngine : class, IFetchEngine<TEntity>
    {
        protected override string InitialUrl => initialUrl;

        protected sealed override bool ValidateResponse(TRawEntity rawEntity) => rawEntity.Entities is { Count: not 0 };

        protected sealed override string? NextUrl(TRawEntity? rawEntity) => rawEntity?.NextUrl;

        protected sealed override IEnumerator<TEntity>? GetNewEnumerator(TRawEntity? rawEntity) => (rawEntity?.Entities as IEnumerable<TEntity>)?.GetEnumerator();
    }

    public class User<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : BaseRecursivePixivAsyncEnumerator<User, PixivUserResponse, TFetchEngine>(pixivFetchEngine, MakoApiKind.AppApi, initialUrl)
        where TFetchEngine : class, IFetchEngine<User>;

    public class Illustration<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : BaseRecursivePixivAsyncEnumerator<Illustration, PixivIllustrationResponse, TFetchEngine>(pixivFetchEngine, MakoApiKind.AppApi, initialUrl)
        where TFetchEngine : class, IFetchEngine<Illustration>;

    public class Novel<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : BaseRecursivePixivAsyncEnumerator<Novel, PixivNovelResponse, TFetchEngine>(pixivFetchEngine, MakoApiKind.AppApi, initialUrl)
        where TFetchEngine : class, IFetchEngine<Novel>;

    public class Comment<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : BaseRecursivePixivAsyncEnumerator<Comment, PixivCommentResponse, TFetchEngine>(pixivFetchEngine, MakoApiKind.AppApi, initialUrl)
        where TFetchEngine : class, IFetchEngine<Comment>;

    public class BookmarkTag<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : BaseRecursivePixivAsyncEnumerator<BookmarkTag, PixivBookmarkTagResponse, TFetchEngine>(pixivFetchEngine, MakoApiKind.AppApi, initialUrl)
        where TFetchEngine : class, IFetchEngine<BookmarkTag>;

    public class Spotlight<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : BaseRecursivePixivAsyncEnumerator<Spotlight, PixivSpotlightResponse, TFetchEngine>(pixivFetchEngine, MakoApiKind.AppApi, initialUrl)
        where TFetchEngine : class, IFetchEngine<Spotlight>;
}
