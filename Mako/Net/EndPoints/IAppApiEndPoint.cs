// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Net.Http;
using System.Threading.Tasks;
using Mako.Engine.Implements;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Net.Request;
using Mako.Net.Response;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Mako.Net.EndPoints;

/// <summary>
/// 方法上 [LoggingFilter] 输出日志
/// </summary>
[HttpHost(MakoHttpOptions.AppApiBaseUrl)]
[OAuthToken]
internal interface IAppApiEndPoint
{
    [HttpPost("/v2/illust/bookmark/add")]
    Task<HttpResponseMessage> AddIllustrationBookmarkAsync([FormContent] AddIllustrationBookmarkRequest request);

    [HttpPost("/v1/illust/bookmark/delete")]
    Task<HttpResponseMessage> RemoveIllustrationBookmarkAsync([FormField] [AliasAs("illust_id")] long id);

    [HttpPost("/v2/novel/bookmark/add")]
    Task<HttpResponseMessage> AddNovelBookmarkAsync([FormContent] AddNovelBookmarkRequest request);

    [HttpPost("/v1/novel/bookmark/delete")]
    Task<HttpResponseMessage> RemoveNovelBookmarkAsync([FormField][AliasAs("novel_id")] long id);

    [HttpPost("/v1/watchlist/manga/add")]
    Task<HttpResponseMessage> AddMangaSeriesWatchlistAsync([FormField][AliasAs("series_id")] long id);

    [HttpPost("/v1/watchlist/manga/delete")]
    Task<HttpResponseMessage> RemoveMangaSeriesWatchlistAsync([FormField][AliasAs("series_id")] long id);

    [HttpPost("/v1/watchlist/novel/add")]
    Task<HttpResponseMessage> AddNovelSeriesWatchlistAsync([FormField][AliasAs("series_id")] long id);

    [HttpPost("/v1/watchlist/novel/delete")]
    Task<HttpResponseMessage> RemoveNovelSeriesWatchlistAsync([FormField][AliasAs("series_id")] long id);

    /// <remarks>
    /// 由于“是否收藏”“是否关注”字段需要实时更新，故不缓存
    /// </remarks>
    [HttpGet("/v1/illust/detail")]
    Task<SingleIllustrationResponse> GetSingleIllustrationAsync([AliasAs("illust_id")] long id, TargetFilter filter);

    /// <inheritdoc cref="GetSingleIllustrationAsync" />
    [HttpGet("/v1/user/detail")]
    Task<SingleUserResponse> GetSingleUserAsync([AliasAs("user_id")] long id, TargetFilter filter);

    /// <inheritdoc cref="GetSingleIllustrationAsync" />
    [HttpGet("/v2/novel/detail")]
    Task<SingleNovelResponse> GetSingleNovelAsync([AliasAs("novel_id")] long id, TargetFilter filter);

    [HttpGet("/v1/illust-series/illust")]
    Task<MangaSeriesContext> GetMangaSeriesContextAsync([AliasAs("illust_id")] long id, TargetFilter filter);

    [HttpGet(MangaSeriesDetailEngine.UrlSegment)]
    Task<MangaSeriesDetailResponse> GetMangaSeriesDetailAsync([AliasAs("illust_series_id")] long id, TargetFilter filter);

    [HttpGet(NovelSeriesDetailEngine.UrlSegment)]
    Task<NovelSeriesDetailResponse> GetNovelSeriesDetailAsync([AliasAs("series_id")] long id);

    [Cache(60 * 1000)]
    [HttpGet("/webview/v2/novel")]
    Task<string> GetNovelContentAsync(long id, bool raw = false);
    /*
    [AliasAs("viewer_version")] string viewerVersion = "20221031_ai",
    [AliasAs("font")] string x1 = "mincho",
    [AliasAs("font_size")] string x2 = "1.0em",
    [AliasAs("line_height")] string x3 = "1.8",
    [AliasAs("color")] string x4 = "#1F1F1F",
    [AliasAs("background_color")] string x5 = "#FFFFFF",
    [AliasAs("mode")] string x6 = "horizontal",
    [AliasAs("theme")] string x7 = "light",
    [AliasAs("margin_top")] string x8 = "60px",
    [AliasAs("margin_bottom")] string x9 = "50px"
    */

    [HttpGet("/v1/user/related")]
    Task<RelatedUsersResponse> RelatedUserAsync([AliasAs("seed_user_id")] long userId, TargetFilter filter);

    [HttpPost("/v1/user/follow/add")]
    Task<HttpResponseMessage> FollowUserAsync([FormContent] FollowUserRequest request);

    [HttpPost("/v1/user/follow/delete")]
    Task<HttpResponseMessage> RemoveFollowUserAsync([FormField][AliasAs("user_id")] long userId);

    [HttpGet("/v1/trending-tags/illust")]
    Task<TrendingTagResponse> GetIllustrationTrendingTagsAsync(TargetFilter filter);

    [HttpGet("/v1/trending-tags/novel")]
    Task<TrendingTagResponse> GetNovelTrendingTagsAsync(TargetFilter filter);

    [Cache(60 * 1000)]
    [HttpGet("/v1/ugoira/metadata")]
    Task<UgoiraMetadataResponse> GetUgoiraMetadataAsync([AliasAs("illust_id")] long id);

    [HttpGet("/v2/search/autocomplete")]
    Task<AutoCompletionResponse> GetAutoCompletionAsync(string word, [AliasAs("merge_plain_keyword_results")] bool mergePlainKeywordResult = true);

    [HttpPost("/v1/illust/comment/add")]
    Task<PostCommentResponse> AddIllustrationCommentAsync([FormContent] AddNormalIllustrationCommentRequest request);

    [HttpPost("/v1/illust/comment/add")]
    Task<PostCommentResponse> AddIllustrationCommentAsync([FormContent] AddStampIllustrationCommentRequest request);

    [HttpPost("/v1/illust/comment/delete")]
    Task<HttpResponseMessage> DeleteIllustrationCommentAsync([FormField][AliasAs("comment_id")] long commentId);

    [HttpPost("/v1/novel/comment/add")]
    Task<PostCommentResponse> AddNovelCommentAsync([FormContent] AddNormalNovelCommentRequest request);

    [HttpPost("/v1/novel/comment/add")]
    Task<PostCommentResponse> AddNovelCommentAsync([FormContent] AddStampNovelCommentRequest request);

    [HttpPost("/v1/novel/comment/delete")]
    Task<HttpResponseMessage> DeleteNovelCommentAsync([FormField][AliasAs("comment_id")] long commentId);

    [HttpGet("/v1/user/ai-show-settings")]
    Task<ShowAiSettingsResponse> GetAiShowSettingsAsync();

    [HttpPost("/v1/user/ai-show-settings/edit")]
    Task<HttpResponseMessage> PostAiShowSettingsAsync([FormField][AliasAs("show_ai")] bool showAi);

    [HttpGet("/v1/user/restricted-mode-settings")]
    Task<RestrictedModeSettingsResponse> GetRestrictedModeSettingsAsync();

    [HttpPost("/v1/user/restricted-mode-settings")]
    Task<HttpResponseMessage> PostRestrictedModeSettingsAsync([FormField][AliasAs("is_restricted_mode_enabled")] bool isRestrictedModeEnabled);

    [HttpGet("/v1/search/options")]
    Task<SearchOptions> GetSearchOptionsAsync(/*和搜索参数一样，但参数没意义*/);
}
