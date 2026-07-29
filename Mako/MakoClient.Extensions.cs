// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Mako.Engine;
using Mako.Engine.Implements;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Net.EndPoints;
using Mako.Net.Requests;
using Mako.Net.Responses;
using Misaki;

namespace Mako;

public partial class MakoClient
{
    /// <inheritdoc cref="IAppApiEndPoint.GetSingleIllustrationAsync" />
    public Task<Illustration> GetIllustrationFromIdAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync<Illustration, SingleIllustrationResponse>(t => t
            .GetSingleIllustrationAsync(id, Configuration.TargetFilter, token));

    /// <inheritdoc cref="IAppApiEndPoint.GetSingleNovelAsync" />
    public Task<Novel> GetNovelFromIdAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync<Novel, SingleNovelResponse>(t => t
            .GetSingleNovelAsync(id, Configuration.TargetFilter, token));

    /// <inheritdoc cref="IAppApiEndPoint.GetSingleIllustrationAsync" />
    public Task<IWorkEntry> GetWorkFromIdAsync(SimpleWorkType type, long id, CancellationToken token = default)
        => RunWithLoggerAsync<IWorkEntry, ISingleResultResponse<IWorkEntry>>(async t => type is SimpleWorkType.Illustration
            ? await t.GetSingleIllustrationAsync(id, Configuration.TargetFilter, token).ConfigureAwait(false)
            : await t.GetSingleNovelAsync(id, Configuration.TargetFilter, token).ConfigureAwait(false));

    /// <inheritdoc cref="IAppApiEndPoint.GetSingleUserAsync" />
    public Task<SingleUserResponse> GetUserFromIdAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync<SingleUserResponse>(t => t
            .GetSingleUserAsync(id, Configuration.TargetFilter, token));

    /// <inheritdoc cref="IAppApiEndPoint.GetAutoCompletionAsync" />
    public Task<IReadOnlyList<Tag>> GetAutoCompletionForKeyword(string word, bool mergePlainKeywordResult = true, CancellationToken token = default)
        => RunWithLoggerAsync<IReadOnlyList<Tag>, AutoCompletionResponse>(t => t
            .GetAutoCompletionAsync(word, mergePlainKeywordResult, token));

    /// <inheritdoc cref="IAppApiEndPoint.GetNovelContentAsync" />
    public Task<NovelContent> GetNovelContentAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync(async t =>
        {
            var contentHtml = await t
                .GetNovelContentAsync(id, false, token)
                .ConfigureAwait(false);

            var leftStack = -2;
            var rightStack = 0;
            var startIndex = -1;
            var endIndex = -1;
            var skipBrace = 1;

            for (var i = 0; i < contentHtml.Length; ++i)
            {
                if (contentHtml[i] is '{')
                {
                    ++leftStack;
                    if (leftStack < 3)
                        startIndex = i;
                }
                else if (contentHtml[i] is '}')
                {
                    ++rightStack;
                    if (rightStack == leftStack)
                    {
                        endIndex = i + 1;
                        if (skipBrace is 0)
                            break;
                        --skipBrace;
                    }
                }
            }

            var span = contentHtml[startIndex..endIndex];

            return JsonSerializer.Deserialize(span, MakoJsonSerializerContext.Default.NovelContent)!;
        });

    /// <inheritdoc cref="IAppApiEndPoint.AddIllustrationBookmarkAsync" />
    public Task<bool> PostWorkBookmarkAsync(SimpleWorkType type, long id, PrivacyPolicy privacyPolicy, IReadOnlyCollection<string>? tags = null, CancellationToken token = default) =>
        RunWithLoggerAsync(t =>
        {
            var urlTags = tags is { Count: > 0 } ? string.Join(' ', tags) : null;
            return type is SimpleWorkType.Illustration
                ? t.AddIllustrationBookmarkAsync(new(privacyPolicy, id, urlTags), token)
                : t.AddNovelBookmarkAsync(new(privacyPolicy, id, urlTags), token);
        });

    /// <inheritdoc cref="IAppApiEndPoint.RemoveIllustrationBookmarkAsync" />
    public Task<bool> RemoveWorkBookmarkAsync(SimpleWorkType type, long id, CancellationToken token = default)
        => RunWithLoggerAsync(t => type is SimpleWorkType.Illustration
            ? t.RemoveIllustrationBookmarkAsync(id, token)
            : t.RemoveNovelBookmarkAsync(id, token));

    /// <inheritdoc cref="IAppApiEndPoint.GetIllustrationBookmarkDetailAsync" />
    public Task<BookmarkDetail> GetWorkBookmarkDetailAsync(SimpleWorkType type, long id, CancellationToken token = default)
        => RunWithLoggerAsync<BookmarkDetail, ISingleResultResponse<BookmarkDetail>>(async t => type is SimpleWorkType.Illustration
            ? await t.GetIllustrationBookmarkDetailAsync(id, token).ConfigureAwait(false)
            : await t.GetNovelBookmarkDetailAsync(id, token).ConfigureAwait(false));

    /// <inheritdoc cref="IAppApiEndPoint.RelatedUserAsync" />
    public Task<IReadOnlyList<User>> RelatedUserAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync<IReadOnlyList<User>, RelatedUsersResponse>(t => t
            .RelatedUserAsync(id, Configuration.TargetFilter, token));

    /// <inheritdoc cref="IAppApiEndPoint.FollowUserAsync" />
    public Task<bool> PostFollowUserAsync(long id, PrivacyPolicy privacyPolicy, CancellationToken token = default)
        => RunWithLoggerAsync(t => t
            .FollowUserAsync(new FollowUserRequest(id, privacyPolicy), token));

    /// <inheritdoc cref="IAppApiEndPoint.RemoveFollowUserAsync" />
    public Task<bool> RemoveFollowUserAsync(long id, CancellationToken token = default) 
        => RunWithLoggerAsync(t => t
            .RemoveFollowUserAsync(id, token));

    /// <inheritdoc cref="IAppApiEndPoint.GetIllustrationTrendingTagsAsync" />
    public Task<IReadOnlyList<TrendingTag>> GetWorkTrendingTagsAsync(SimpleWorkType type, CancellationToken token = default)
        => RunWithLoggerAsync<IReadOnlyList<TrendingTag>, TrendingTagResponse>(t => type is SimpleWorkType.Illustration
            ? t.GetIllustrationTrendingTagsAsync(Configuration.TargetFilter, token)
            : t.GetNovelTrendingTagsAsync(Configuration.TargetFilter, token));

    /// <inheritdoc cref="IAppApiEndPoint.GetUgoiraMetadataAsync" />
    public Task<UgoiraMetadata> GetUgoiraMetadataAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync<UgoiraMetadata, UgoiraMetadataResponse>(t => t
            .GetUgoiraMetadataAsync(id, token));

    /// <inheritdoc cref="IAppApiEndPoint.DeleteIllustrationCommentAsync" />
    public Task<bool> DeleteWorkCommentAsync(SimpleWorkType type, long commentId, CancellationToken token = default)
        => RunWithLoggerAsync(t => type is SimpleWorkType.Illustration
            ? t.DeleteIllustrationCommentAsync(commentId, token)
            : t.DeleteNovelCommentAsync(commentId, token));

    /// <inheritdoc cref="IAppApiEndPoint.AddIllustrationCommentAsync(AddNormalIllustrationCommentRequest, CancellationToken" />
    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, string content, CancellationToken token = default)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddNormalIllustrationCommentRequest(workId, null, content), token)
            : t.AddNovelCommentAsync(new AddNormalNovelCommentRequest(workId, null, content), token));

    /// <inheritdoc cref="IAppApiEndPoint.AddIllustrationCommentAsync(AddStampIllustrationCommentRequest, CancellationToken" />
    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, int stampId, CancellationToken token = default)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddStampIllustrationCommentRequest(workId, null, stampId), token)
            : t.AddNovelCommentAsync(new AddStampNovelCommentRequest(workId, null, stampId), token));

    /// <inheritdoc cref="IAppApiEndPoint.AddIllustrationCommentAsync(AddNormalIllustrationCommentRequest, CancellationToken" />
    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, long parentCommentId, string content, CancellationToken token = default)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddNormalIllustrationCommentRequest(workId, parentCommentId, content), token)
            : t.AddNovelCommentAsync(new AddNormalNovelCommentRequest(workId, parentCommentId, content), token));

    /// <inheritdoc cref="IAppApiEndPoint.AddIllustrationCommentAsync(AddStampIllustrationCommentRequest, CancellationToken" />
    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, long parentCommentId, int stampId, CancellationToken token = default)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddStampIllustrationCommentRequest(workId, parentCommentId, stampId), token)
            : t.AddNovelCommentAsync(new AddStampNovelCommentRequest(workId, parentCommentId, stampId), token));
    
    /// <inheritdoc cref="IAppApiEndPoint.GetAiShowSettingsAsync" />
    public Task<bool> GetAiShowSettingsAsync(CancellationToken token = default)
        => RunWithLoggerAsync<bool, ShowAiSettingsResponse>(t => t.GetAiShowSettingsAsync(token));

    /// <inheritdoc cref="IAppApiEndPoint.PostAiShowSettingsAsync" />
    public Task<bool> PostAiShowSettingsAsync(bool showAi, CancellationToken token = default)
        => RunWithLoggerAsync<bool, ShowAiSettingsResponse>(t => t.PostAiShowSettingsAsync(new(showAi), token));

    /// <inheritdoc cref="IAppApiEndPoint.GetRestrictedModeSettingsAsync" />
    public Task<bool> GetRestrictedModeSettingsAsync(CancellationToken token = default)
        => RunWithLoggerAsync<bool, RestrictedModeSettingsResponse>(t => t.GetRestrictedModeSettingsAsync(token));

    /// <inheritdoc cref="IAppApiEndPoint.PostRestrictedModeSettingsAsync" />
    public Task<bool> PostRestrictedModeSettingsAsync(bool isRestrictedModeEnabled, CancellationToken token = default)
        => RunWithLoggerAsync<bool, RestrictedModeSettingsResponse>(t => t.PostRestrictedModeSettingsAsync(new(isRestrictedModeEnabled), token));

    /// <inheritdoc cref="IAppApiEndPoint.AddMangaSeriesWatchlistAsync" />
    public Task<bool> PostWorkSeriesWatchlistAsync(SimpleWorkType type, long id, CancellationToken token = default) =>
        RunWithLoggerAsync(t => type is SimpleWorkType.Illustration
            ? t.AddMangaSeriesWatchlistAsync(id, token)
            : t.AddNovelSeriesWatchlistAsync(id, token));

    /// <inheritdoc cref="IAppApiEndPoint.RemoveMangaSeriesWatchlistAsync" />
    public Task<bool> RemoveWorkSeriesWatchlistAsync(SimpleWorkType type, long id, CancellationToken token = default)
        => RunWithLoggerAsync(t => type is SimpleWorkType.Illustration
            ? t.RemoveMangaSeriesWatchlistAsync(id, token)
            : t.RemoveNovelSeriesWatchlistAsync(id, token));

    /// <inheritdoc cref="IAppApiEndPoint.GetSearchOptionsAsync" />
    public Task<SearchOptions> GetSearchOptionsAsync(CancellationToken token = default)
        => RunWithLoggerAsync(t => t.GetSearchOptionsAsync(token));

    /// <inheritdoc cref="IAppApiEndPoint.GetMangaSeriesContextAsync" />
    /// <remarks>
    /// 对标 <see cref="GetNovelContentAsync"/>（<see cref="NovelContent"/> 有系列信息）
    /// </remarks>
    public Task<MangaSeriesContextResponse> GetMangaSeriesContextAsync(long seriesId, CancellationToken token = default)
        => RunWithLoggerAsync(t => t.GetMangaSeriesContextAsync(seriesId, Configuration.TargetFilter, token));

    /// <inheritdoc cref="IAppApiEndPoint.GetMangaSeriesDetailAsync" />
    public async Task<(MangaSeriesDetail Detail, Illustration First, IFetchEngine<Illustration> Engine)> GetMangaSeriesAsync(long seriesId, CancellationToken token = default)
    {
        var response = await RunWithLoggerAsync(t => t.GetMangaSeriesDetailAsync(seriesId, Configuration.TargetFilter, token)).ConfigureAwait(false);
        return (response.SeriesDetail, response.First, new MangaSeriesEngine(this, seriesId, response));
    }

    /// <inheritdoc cref="IAppApiEndPoint.GetNovelSeriesDetailAsync" />
    public async Task<(NovelSeriesDetail Detail, Novel First, Novel Latest, IFetchEngine<Novel> Engine)> GetNovelSeriesAsync(long seriesId, CancellationToken token = default)
    {
        var response = await RunWithLoggerAsync(t => t.GetNovelSeriesDetailAsync(seriesId, token)).ConfigureAwait(false);
        return (response.SeriesDetail, response.First, response.Latest, new NovelSeriesEngine(this, seriesId, response));
    }

    /// <inheritdoc cref="IAppApiEndPoint.GetMangaSeriesDetailAsync" />
    public async Task<(SeriesDetailBase Detail, IWorkEntry First, IFetchEngine<IWorkEntry> Engine)> GetWorkSeriesAsync(SimpleWorkType type, long seriesId, CancellationToken token = default)
    {
        if (type is SimpleWorkType.Novel)
        {
            var response = await GetNovelSeriesAsync(seriesId, token).ConfigureAwait(false);
            return (response.Detail, response.First, response.Engine);
        }
        else
        {
            var response = await GetMangaSeriesAsync(seriesId, token).ConfigureAwait(false);
            return (response.Detail, response.First, response.Engine);
        }
    }

    #region Misaki

    async Task<IArtworkInfo> IGetArtworkService.GetArtworkAsync(string id, CancellationToken token) => await GetIllustrationFromIdAsync(long.Parse(id), token).ConfigureAwait(false);

    async Task<bool> IPostFavoriteService.PostFavoriteAsync(string id, bool favorite, CancellationToken token)
    {
        var l = long.Parse(id);
        try
        {
            if (favorite)
                return await PostWorkBookmarkAsync(SimpleWorkType.Illustration, l, PrivacyPolicy.Public, token: token).ConfigureAwait(false);

            return !await RemoveWorkBookmarkAsync(SimpleWorkType.Illustration, l, token).ConfigureAwait(false);
        }
        catch
        {
            return !favorite;
        }
    }

    #endregion
}
