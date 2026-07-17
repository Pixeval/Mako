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
using Mako.Net.Requests;
using Mako.Net.Responses;
using Misaki;

namespace Mako;

public partial class MakoClient
{
    public Task<Illustration> GetIllustrationFromIdAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync<Illustration, SingleIllustrationResponse>(t => t
            .GetSingleIllustrationAsync(id, Configuration.TargetFilter, token));

    public Task<Novel> GetNovelFromIdAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync<Novel, SingleNovelResponse>(t => t
            .GetSingleNovelAsync(id, Configuration.TargetFilter, token));

    public Task<IWorkEntry> GetWorkFromIdAsync(SimpleWorkType type, long id, CancellationToken token = default)
        => RunWithLoggerAsync<IWorkEntry, ISingleResultResponse<IWorkEntry>>(async t => type is SimpleWorkType.Illustration
            ? await t.GetSingleIllustrationAsync(id, Configuration.TargetFilter, token).ConfigureAwait(false)
            : await t.GetSingleNovelAsync(id, Configuration.TargetFilter, token).ConfigureAwait(false));

    public Task<SingleUserResponse> GetUserFromIdAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync<SingleUserResponse>(t => t
            .GetSingleUserAsync(id, Configuration.TargetFilter, token));

    public Task<IReadOnlyList<Tag>> GetAutoCompletionForKeyword(string word, bool mergePlainKeywordResult = true, CancellationToken token = default)
        => RunWithLoggerAsync<IReadOnlyList<Tag>, AutoCompletionResponse>(t => t
            .GetAutoCompletionAsync(word, mergePlainKeywordResult, token));

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

    public Task<bool> PostWorkBookmarkAsync(SimpleWorkType type, long id, PrivacyPolicy privacyPolicy, IReadOnlyCollection<string>? tags = null, CancellationToken token = default) =>
        RunWithLoggerAsync(t =>
        {
            var urlTags = tags is { Count: > 0 } ? string.Join(' ', tags) : null;
            return type is SimpleWorkType.Illustration
                ? t.AddIllustrationBookmarkAsync(new(privacyPolicy, id, urlTags), token)
                : t.AddNovelBookmarkAsync(new(privacyPolicy, id, urlTags), token);
        });

    public Task<bool> RemoveWorkBookmarkAsync(SimpleWorkType type, long id, CancellationToken token = default)
        => RunWithLoggerAsync(t => type is SimpleWorkType.Illustration
            ? t.RemoveIllustrationBookmarkAsync(id, token)
            : t.RemoveNovelBookmarkAsync(id, token));

    public Task<IReadOnlyList<User>> RelatedUserAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync<IReadOnlyList<User>, RelatedUsersResponse>(t => t
            .RelatedUserAsync(id, Configuration.TargetFilter, token));

    public Task<bool> PostFollowUserAsync(long id, PrivacyPolicy privacyPolicy, CancellationToken token = default)
        => RunWithLoggerAsync(t => t
            .FollowUserAsync(new FollowUserRequest(id, privacyPolicy), token));

    public Task<bool> RemoveFollowUserAsync(long id, CancellationToken token = default) 
        => RunWithLoggerAsync(t => t
            .RemoveFollowUserAsync(id, token));

    public Task<IReadOnlyList<TrendingTag>> GetWorkTrendingTagsAsync(SimpleWorkType type, CancellationToken token = default)
        => RunWithLoggerAsync<IReadOnlyList<TrendingTag>, TrendingTagResponse>(t => type is SimpleWorkType.Illustration
            ? t.GetIllustrationTrendingTagsAsync(Configuration.TargetFilter, token)
            : t.GetNovelTrendingTagsAsync(Configuration.TargetFilter, token));

    public Task<UgoiraMetadata> GetUgoiraMetadataAsync(long id, CancellationToken token = default)
        => RunWithLoggerAsync<UgoiraMetadata, UgoiraMetadataResponse>(t => t
            .GetUgoiraMetadataAsync(id, token));

    public Task<bool> DeleteWorkCommentAsync(SimpleWorkType type, long commentId, CancellationToken token = default)
        => RunWithLoggerAsync(t => type is SimpleWorkType.Illustration
            ? t.DeleteIllustrationCommentAsync(commentId, token)
            : t.DeleteNovelCommentAsync(commentId, token));

    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, string content, CancellationToken token = default)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddNormalIllustrationCommentRequest(workId, null, content), token)
            : t.AddNovelCommentAsync(new AddNormalNovelCommentRequest(workId, null, content), token));

    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, int stampId, CancellationToken token = default)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddStampIllustrationCommentRequest(workId, null, stampId), token)
            : t.AddNovelCommentAsync(new AddStampNovelCommentRequest(workId, null, stampId), token));

    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, long parentCommentId, string content, CancellationToken token = default)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddNormalIllustrationCommentRequest(workId, parentCommentId, content), token)
            : t.AddNovelCommentAsync(new AddNormalNovelCommentRequest(workId, parentCommentId, content), token));

    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, long parentCommentId, int stampId, CancellationToken token = default)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddStampIllustrationCommentRequest(workId, parentCommentId, stampId), token)
            : t.AddNovelCommentAsync(new AddStampNovelCommentRequest(workId, parentCommentId, stampId), token));

    public Task<bool> GetAiShowSettingsAsync(CancellationToken token = default)
        => RunWithLoggerAsync<bool, ShowAiSettingsResponse>(t => t.GetAiShowSettingsAsync(token));

    public Task<bool> PostAiShowSettingsAsync(bool showAi, CancellationToken token = default)
        => RunWithLoggerAsync<bool, ShowAiSettingsResponse>(t => t.PostAiShowSettingsAsync(new(showAi), token));

    public Task<bool> GetRestrictedModeSettingsAsync(CancellationToken token = default)
        => RunWithLoggerAsync<bool, RestrictedModeSettingsResponse>(t => t.GetRestrictedModeSettingsAsync(token));

    public Task<bool> PostRestrictedModeSettingsAsync(bool isRestrictedModeEnabled, CancellationToken token = default)
        => RunWithLoggerAsync<bool, RestrictedModeSettingsResponse>(t => t.PostRestrictedModeSettingsAsync(new(isRestrictedModeEnabled), token));

    public Task<bool> PostWorkSeriesWatchlistAsync(SimpleWorkType type, long id, CancellationToken token = default) =>
        RunWithLoggerAsync(t => type is SimpleWorkType.Novel
            ? t.AddNovelSeriesWatchlistAsync(id, token)
            : t.AddMangaSeriesWatchlistAsync(id, token));

    public Task<bool> RemoveWorkSeriesWatchlistAsync(SimpleWorkType type, long id, CancellationToken token = default)
        => RunWithLoggerAsync(t => type is SimpleWorkType.Novel
            ? t.RemoveNovelSeriesWatchlistAsync(id, token)
            : t.RemoveMangaSeriesWatchlistAsync(id, token));

    public Task<SearchOptions> GetSearchOptionsAsync(CancellationToken token = default)
        => RunWithLoggerAsync(t => t.GetSearchOptionsAsync(token));

    /// <remarks>
    /// 对标 <see cref="GetNovelContentAsync"/>（<see cref="NovelContent"/> 有系列信息）
    /// </remarks>
    public Task<MangaSeriesContextResponse> GetMangaSeriesContextAsync(long seriesId, CancellationToken token = default)
        => RunWithLoggerAsync(t => t.GetMangaSeriesContextAsync(seriesId, Configuration.TargetFilter, token));

    public async Task<(MangaSeriesDetail Detail, Illustration First, IFetchEngine<Illustration> Engine)> GetMangaSeriesAsync(long seriesId, CancellationToken token = default)
    {
        var response = await RunWithLoggerAsync(t => t.GetMangaSeriesDetailAsync(seriesId, Configuration.TargetFilter, token)).ConfigureAwait(false);
        return (response.SeriesDetail, response.First, new MangaSeriesEngine(this, seriesId, response));
    }

    public async Task<(NovelSeriesDetail Detail, Novel First, Novel Latest, IFetchEngine<Novel> Engine)> GetNovelSeriesAsync(long seriesId, CancellationToken token = default)
    {
        var response = await RunWithLoggerAsync(t => t.GetNovelSeriesDetailAsync(seriesId, token)).ConfigureAwait(false);
        return (response.SeriesDetail, response.First, response.Latest, new NovelSeriesEngine(this, seriesId, response));
    }

    public async Task<(SeriesDetailBase Detail, IWorkEntry First, IFetchEngine<IWorkEntry>)> GetWorkSeriesAsync(SimpleWorkType type, long seriesId, CancellationToken token = default)
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
