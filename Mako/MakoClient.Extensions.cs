// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Mako.Engine;
using Mako.Engine.Implements;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Net.EndPoints;
using Mako.Net.Request;
using Mako.Net.Response;
using Microsoft.Extensions.DependencyInjection;
using Misaki;
using WebApiClientCore.Parameters;

namespace Mako;

public partial class MakoClient
{
    /// <summary>
    /// Gets the detail of an illustration from the illustration id
    /// </summary>
    /// <param name="id">The illustration id</param>
    /// <returns></returns>
    public Task<Illustration> GetIllustrationFromIdAsync(long id)
        => RunWithLoggerAsync<Illustration, SingleIllustrationResponse>(t => t
            .GetSingleIllustrationAsync(id, Configuration.TargetFilter));

    async Task<IArtworkInfo> IGetArtworkService.GetArtworkAsync(string id) => await GetIllustrationFromIdAsync(long.Parse(id)).ConfigureAwait(false);

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

    /// <summary>
    /// Sends a request to the Pixiv to add it to the bookmark
    /// </summary>
    /// <param name="id">The ID of the illustration which needs to be bookmarked</param>
    /// <param name="privacyPolicy">Indicates the privacy of the illustration in the bookmark</param>
    /// <param name="tags"></param>
    public Task<bool> PostIllustrationBookmarkAsync(long id, PrivacyPolicy privacyPolicy, IEnumerable<string>? tags = null) =>
        RunWithLoggerAsync(t =>
        {
            var urlTags = tags is null ? null : string.Join(' ', tags);
            return t.AddIllustrationBookmarkAsync(new AddIllustrationBookmarkRequest(privacyPolicy, id, urlTags));
        });

    async Task<bool> IPostFavoriteService.PostFavoriteAsync(string id, bool favorite)
    {
        var l = long.Parse(id);
        try
        {
            if (favorite)
                return await PostIllustrationBookmarkAsync(l, PrivacyPolicy.Public).ConfigureAwait(false);

            return !await RemoveIllustrationBookmarkAsync(l).ConfigureAwait(false);
        }
        catch
        {
            return !favorite;
        }
    }

    /// <summary>
    /// Sends a request to the Pixiv to remove it from the bookmark
    /// </summary>
    /// <param name="id">The ID of the illustration which needs to be removed from the bookmark</param>
    /// <returns>A <see cref="Task" /> represents the operation</returns>
    public Task<bool> RemoveIllustrationBookmarkAsync(long id)
        => RunWithLoggerAsync(t => t
            .RemoveIllustrationBookmarkAsync(id));

    public Task<bool> PostNovelBookmarkAsync(long id, PrivacyPolicy privacyPolicy, IEnumerable<string>? tags = null) =>
        RunWithLoggerAsync(t =>
        {
            var urlTags = tags is null ? null : string.Join(' ', tags);
            return t.AddNovelBookmarkAsync(new AddNovelBookmarkRequest(privacyPolicy, id, urlTags));
        });

    public Task<bool> RemoveNovelBookmarkAsync(long id)
        => RunWithLoggerAsync(t => t
            .RemoveNovelBookmarkAsync(id));

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

    public Task<IReadOnlyList<User>> RelatedUserAsync(long id)
        => RunWithLoggerAsync<IReadOnlyList<User>, RelatedUsersResponse>(t => t
            .RelatedUserAsync(id, Configuration.TargetFilter));

    public Task<bool> PostFollowUserAsync(long id, PrivacyPolicy privacyPolicy)
        => RunWithLoggerAsync(t => t
            .FollowUserAsync(new FollowUserRequest(id, privacyPolicy)));

    public Task<bool> RemoveFollowUserAsync(long id)
        => RunWithLoggerAsync(t => t
            .RemoveFollowUserAsync(id));

    public Task<IReadOnlyList<TrendingTag>> GetIllustrationTrendingTagsAsync()
        => RunWithLoggerAsync<IReadOnlyList<TrendingTag>, TrendingTagResponse>(t => t
            .GetIllustrationTrendingTagsAsync(Configuration.TargetFilter));

    public Task<IReadOnlyList<TrendingTag>> GetNovelTrendingTagsAsync()
        => RunWithLoggerAsync<IReadOnlyList<TrendingTag>, TrendingTagResponse>(t => t
            .GetNovelTrendingTagsAsync(Configuration.TargetFilter));

    public Task<UgoiraMetadata> GetUgoiraMetadataAsync(long id)
        => RunWithLoggerAsync<UgoiraMetadata, UgoiraMetadataResponse>(t => t
            .GetUgoiraMetadataAsync(id));

    public Task<bool> DeleteIllustrationCommentAsync(long commentId)
        => RunWithLoggerAsync(t => t
            .DeleteIllustrationCommentAsync(commentId));

    public Task<bool> DeleteNovelCommentAsync(long commentId)
        => RunWithLoggerAsync(t => t
            .DeleteNovelCommentAsync(commentId));

    public Task<Comment> AddIllustrationCommentAsync(long illustrationId, string content)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => t
            .AddIllustrationCommentAsync(new AddNormalIllustrationCommentRequest(illustrationId, null, content)));

    public Task<Comment> AddIllustrationCommentAsync(long illustrationId, int stampId)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => t
            .AddIllustrationCommentAsync(new AddStampIllustrationCommentRequest(illustrationId, null, stampId)));

    public Task<Comment> AddIllustrationCommentAsync(long illustrationId, long parentCommentId, string content)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => t
            .AddIllustrationCommentAsync(new AddNormalIllustrationCommentRequest(illustrationId, parentCommentId, content)));

    public Task<Comment> AddIllustrationCommentAsync(long illustrationId, long parentCommentId, int stampId)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => t
            .AddIllustrationCommentAsync(new AddStampIllustrationCommentRequest(illustrationId, parentCommentId, stampId)));

    public Task<Comment> AddNovelCommentAsync(long novelId, string content)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => t
            .AddNovelCommentAsync(new AddNormalNovelCommentRequest(novelId, null, content)));

    public Task<Comment> AddNovelCommentAsync(long novelId, int stampId)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => t
            .AddNovelCommentAsync(new AddStampNovelCommentRequest(novelId, null, stampId)));

    public Task<Comment> AddNovelCommentAsync(long novelId, long parentCommentId, string content)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => t
            .AddNovelCommentAsync(new AddNormalNovelCommentRequest(novelId, parentCommentId, content)));

    public Task<Comment> AddNovelCommentAsync(long novelId, long parentCommentId, int stampId)
        => RunWithLoggerAsync<Comment, PostCommentResponse>(t => t
            .AddNovelCommentAsync(new AddStampNovelCommentRequest(novelId, parentCommentId, stampId)));

    public Task<bool> GetAiShowSettingsAsync()
        => RunWithLoggerAsync<bool, ShowAiSettingsResponse>(t => t.GetAiShowSettingsAsync());

    public Task<bool> PostAiShowSettingsAsync(bool showAi)
        => RunWithLoggerAsync(t => t.PostAiShowSettingsAsync(showAi));

    public Task<bool> GetRestrictedModeSettingsAsync()
        => RunWithLoggerAsync<bool, RestrictedModeSettingsResponse>(t => t.GetRestrictedModeSettingsAsync());

    public Task<bool> PostRestrictedModeSettingsAsync(bool isRestrictedModeEnabled)
        => RunWithLoggerAsync(t => t.PostRestrictedModeSettingsAsync(isRestrictedModeEnabled));

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

    public Task<IReadOnlyList<Result>> ReverseSearchAsync(Stream imgStream, string apiKey)
        => RunWithLoggerAsync(async () =>
        {
            var result = await Provider.GetRequiredService<IReverseSearchApiEndPoint>()
                .GetSauceAsync(new FormDataFile(imgStream, "img"), new ReverseSearchRequest(apiKey));
            return result.Header.Status is 0 ? result.Results : [];
        });
}
