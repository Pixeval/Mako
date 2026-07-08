// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json;
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
    public Task<Illustration> GetIllustrationFromIdAsync(long id)
        => RunWithLoggerAsync<Illustration, SingleIllustrationResponse>(t => t
            .GetSingleIllustrationAsync(id, Configuration.TargetFilter));

    public Task<IReadOnlyList<Tag>> GetAutoCompletionForKeyword(string word)
        => RunWithLoggerAsync<IReadOnlyList<Tag>, AutoCompletionResponse>(t => t
            .GetAutoCompletionAsync(word));

    public Task<SingleUserResponse> GetUserFromIdAsync(long id)
        => RunWithLoggerAsync<SingleUserResponse>(t => t
            .GetSingleUserAsync(id, Configuration.TargetFilter));

    public Task<Novel> GetNovelFromIdAsync(long id)
        => RunWithLoggerAsync<Novel, SingleNovelResponse>(t => t
            .GetSingleNovelAsync(id, Configuration.TargetFilter));

    public Task<NovelContent> GetNovelContentAsync(long id)
        => RunWithLoggerAsync(async t =>
        {
            var contentHtml = await t
                .GetNovelContentAsync(id)
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

    public Task<bool> PostWorkBookmarkAsync(SimpleWorkType type, long id, PrivacyPolicy privacyPolicy, IReadOnlyCollection<string>? tags = null) =>
        RunWithLoggerAsync(t =>
        {
            var urlTags = tags is { Count: > 0 } ? string.Join(' ', tags) : null;
            return type is SimpleWorkType.Illustration
                ? t.AddIllustrationBookmarkAsync(new(privacyPolicy, id, urlTags))
                : t.AddNovelBookmarkAsync(new(privacyPolicy, id, urlTags));
        });

    public Task<bool> RemoveWorkBookmarkAsync(SimpleWorkType type, long id)
        => RunWithLoggerAsync(t => type is SimpleWorkType.Illustration
            ? t.RemoveIllustrationBookmarkAsync(id)
            : t.RemoveNovelBookmarkAsync(id));

    public Task<IReadOnlyList<User>> RelatedUserAsync(long id)
        => RunWithLoggerAsync<IReadOnlyList<User>, RelatedUsersResponse>(t => t
            .RelatedUserAsync(id, Configuration.TargetFilter));

    public Task<bool> PostFollowUserAsync(long id, PrivacyPolicy privacyPolicy)
        => RunWithLoggerAsync(t => t
            .FollowUserAsync(new FollowUserRequest(id, privacyPolicy)));

    public Task<bool> RemoveFollowUserAsync(long id)
        => RunWithLoggerAsync(t => t
            .RemoveFollowUserAsync(id));

    public Task<IReadOnlyList<TrendingTag>> GetWorkTrendingTagsAsync(SimpleWorkType type)
        => RunWithLoggerAsync<IReadOnlyList<TrendingTag>, TrendingTagResponse>(t => type is SimpleWorkType.Illustration
            ? t.GetIllustrationTrendingTagsAsync(Configuration.TargetFilter)
            : t.GetNovelTrendingTagsAsync(Configuration.TargetFilter));

    public Task<UgoiraMetadata> GetUgoiraMetadataAsync(long id)
        => RunWithLoggerAsync<UgoiraMetadata, UgoiraMetadataResponse>(t => t
            .GetUgoiraMetadataAsync(id));

    public Task<bool> DeleteWorkCommentAsync(SimpleWorkType type, long commentId)
        => RunWithLoggerAsync(t => type is SimpleWorkType.Illustration
            ? t.DeleteIllustrationCommentAsync(commentId)
            : t.DeleteNovelCommentAsync(commentId));

    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, string content)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddNormalIllustrationCommentRequest(workId, null, content))
            : t.AddNovelCommentAsync(new AddNormalNovelCommentRequest(workId, null, content)));

    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, int stampId)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddStampIllustrationCommentRequest(workId, null, stampId))
            : t.AddNovelCommentAsync(new AddStampNovelCommentRequest(workId, null, stampId)));

    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, long parentCommentId, string content)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddNormalIllustrationCommentRequest(workId, parentCommentId, content))
            : t.AddNovelCommentAsync(new AddNormalNovelCommentRequest(workId, parentCommentId, content)));

    public Task<Comment> AddWorkCommentAsync(SimpleWorkType type, long workId, long parentCommentId, int stampId)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => type is SimpleWorkType.Illustration
            ? t.AddIllustrationCommentAsync(new AddStampIllustrationCommentRequest(workId, parentCommentId, stampId))
            : t.AddNovelCommentAsync(new AddStampNovelCommentRequest(workId, parentCommentId, stampId)));

    public Task<bool> GetAiShowSettingsAsync()
        => RunWithLoggerAsync<bool, ShowAiSettingsResponse>(t => t.GetAiShowSettingsAsync());

    public Task<bool> PostAiShowSettingsAsync(bool showAi)
        => RunWithLoggerAsync<bool, ShowAiSettingsResponse>(t => t.PostAiShowSettingsAsync(new(showAi)));

    public Task<bool> GetRestrictedModeSettingsAsync()
        => RunWithLoggerAsync<bool, RestrictedModeSettingsResponse>(t => t.GetRestrictedModeSettingsAsync());

    public Task<bool> PostRestrictedModeSettingsAsync(bool isRestrictedModeEnabled)
        => RunWithLoggerAsync<bool, RestrictedModeSettingsResponse>(t => t.PostRestrictedModeSettingsAsync(new(isRestrictedModeEnabled)));

    public Task<bool> PostMangaSeriesWatchlistAsync(long id) =>
        RunWithLoggerAsync(t => t
            .AddMangaSeriesWatchlistAsync(id));

    public Task<bool> RemoveMangaSeriesWatchlistAsync(long id)
        => RunWithLoggerAsync(t => t
            .RemoveMangaSeriesWatchlistAsync(id));

    public Task<bool> PostNovelSeriesWatchlistAsync(long id) =>
        RunWithLoggerAsync(t => t
            .AddNovelSeriesWatchlistAsync(id));

    public Task<bool> RemoveNovelSeriesWatchlistAsync(long id)
        => RunWithLoggerAsync(t => t
            .RemoveNovelSeriesWatchlistAsync(id));

    public Task<SearchOptions> GetSearchOptionsAsync()
        => RunWithLoggerAsync(t => t.GetSearchOptionsAsync());

    public Task<MangaSeriesContext> GetMangaSeriesContextAsync(long seriesId)
        => RunWithLoggerAsync(t => t.GetMangaSeriesContextAsync(seriesId, Configuration.TargetFilter));

    public async Task<(MangaSeriesDetail Detail, Illustration First, IFetchEngine<Illustration>)> GetMangaSeriesAsync(long seriesId)
    {
        var response = await RunWithLoggerAsync(t => t.GetMangaSeriesDetailAsync(seriesId, Configuration.TargetFilter)).ConfigureAwait(false);
        return (response.SeriesDetail, response.First, new MangaSeriesDetailEngine(this, seriesId, response));
    }

    public async Task<(NovelSeriesDetail Detail, Novel First, Novel Latest, IFetchEngine<Novel>)> GetNovelSeriesAsync(long seriesId)
    {
        var response = await RunWithLoggerAsync(t => t.GetNovelSeriesDetailAsync(seriesId)).ConfigureAwait(false);
        return (response.SeriesDetail, response.First, response.Latest, new NovelSeriesDetailEngine(this, seriesId, response));
    }

    #region Misaki

    async Task<IArtworkInfo> IGetArtworkService.GetArtworkAsync(string id) => await GetIllustrationFromIdAsync(long.Parse(id)).ConfigureAwait(false);

    async Task<bool> IPostFavoriteService.PostFavoriteAsync(string id, bool favorite)
    {
        var l = long.Parse(id);
        try
        {
            if (favorite)
                return await PostWorkBookmarkAsync(SimpleWorkType.Illustration, l, PrivacyPolicy.Public).ConfigureAwait(false);

            return !await RemoveWorkBookmarkAsync(SimpleWorkType.Illustration, l).ConfigureAwait(false);
        }
        catch
        {
            return !favorite;
        }
    }

    #endregion
}
