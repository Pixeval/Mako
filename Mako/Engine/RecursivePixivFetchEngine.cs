// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Mako.Global.Exception;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Misaki;

namespace Mako.Engine;

internal abstract class RecursivePixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
    : AbstractPixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine>(pixivFetchEngine, MakoApiKind.AppApi)
    where TEntity : class, IMisakiModel
    where TRawEntity : class, IPixivNextUrlResponse<TEntity>
    where TFetchEngine : class, IFetchEngine<TEntity>
{
    private TRawEntity? CurrentEntity { get; set; }

    protected string InitialUrl => initialUrl;

    public override async ValueTask<bool> MoveNextAsync()
    {
        if (IsCancellationRequested || PixivFetchEngine.EngineHandle.IsCompleted)
            return false;

        if (CurrentEntity is null)
        {
            if (await GetJsonResponseAsync(InitialUrl).ConfigureAwait(false) is { } raw)
                Update(raw);
            else
                return false;
        }

        if (CurrentEntityEnumerator!.MoveNext()) // If the enumerator can proceeds then return true
            return true;

        if (string.IsNullOrEmpty(CurrentEntity.NextUrl)) // Check if there are more pages, return false if not
        {
            PixivFetchEngine.EngineHandle.Complete();
            return false;
        }

        if (await GetJsonResponseAsync(CurrentEntity.NextUrl).ConfigureAwait(false) is { } value) // Else request a new page
        {
            if (IsCancellationRequested)
                return false;

            Update(value);
            return CurrentEntityEnumerator.MoveNext();
        }

        // 遇到异常直接停止，但不标记为完成或取消
        return false;
    }

    [MemberNotNull(nameof(CurrentEntity))]
    private void Update(TRawEntity rawEntity)
    {
        CurrentEntity = rawEntity;
        CurrentEntityEnumerator = rawEntity.Entities.GetEnumerator();
        ++PixivFetchEngine.RequestedPages;
    }

    protected async Task<TRawEntity?> GetJsonResponseAsync(string url)
    {
        try
        {
            // Authorization
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var tokenProvider = MakoClient.GetTokenProvider();
            var tokenResult = await tokenProvider.GetTokenAsync().ConfigureAwait(false);
            request.Headers.Authorization = new(tokenResult.Token_type ?? "Bearer", tokenResult.Access_token);

            var responseMessage = await ApiClient.SendAsync(request).ConfigureAwait(false);
            if (!responseMessage.IsSuccessStatusCode)
            {
                MakoClient.LogException(await MakoNetworkException.FromHttpResponseMessageAsync(responseMessage, MakoClient.Configuration.DomainFronting).ConfigureAwait(false));
                return null;
            }

            var json = await responseMessage.Content.ReadFromJsonAsync(typeof(TRawEntity), MakoJsonSerializerContext.Default).ConfigureAwait(false);

            if (json is TRawEntity result)
                return result;

            MakoClient.LogException(new MakoNetworkException(url, MakoClient.Configuration.DomainFronting, "Result is null", (int) responseMessage.StatusCode));
        }
        catch (Exception e)
        {
            MakoClient.LogException(new MakoNetworkException(url, MakoClient.Configuration.DomainFronting, e.Message, (int?) (e as HttpRequestException)?.StatusCode ?? -1));
        }

        return null;
    }
}

internal static class RecursivePixivAsyncEnumerators
{
    public class User<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : RecursivePixivAsyncEnumerator<User, PixivUserResponse, TFetchEngine>(pixivFetchEngine, initialUrl)
        where TFetchEngine : class, IFetchEngine<User>;

    public class Illustration<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : RecursivePixivAsyncEnumerator<Illustration, PixivIllustrationResponse, TFetchEngine>(pixivFetchEngine, initialUrl)
        where TFetchEngine : class, IFetchEngine<Illustration>;

    public class Novel<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : RecursivePixivAsyncEnumerator<Novel, PixivNovelResponse, TFetchEngine>(pixivFetchEngine, initialUrl)
        where TFetchEngine : class, IFetchEngine<Novel>;

    public class Comment<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : RecursivePixivAsyncEnumerator<Comment, PixivCommentResponse, TFetchEngine>(pixivFetchEngine, initialUrl)
        where TFetchEngine : class, IFetchEngine<Comment>;

    public class BookmarkTag<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : RecursivePixivAsyncEnumerator<BookmarkTag, PixivBookmarkTagResponse, TFetchEngine>(pixivFetchEngine, initialUrl)
        where TFetchEngine : class, IFetchEngine<BookmarkTag>;

    public class Spotlight<TFetchEngine>(TFetchEngine pixivFetchEngine, string initialUrl)
        : RecursivePixivAsyncEnumerator<Spotlight, PixivSpotlightResponse, TFetchEngine>(pixivFetchEngine, initialUrl)
        where TFetchEngine : class, IFetchEngine<Spotlight>;
}
