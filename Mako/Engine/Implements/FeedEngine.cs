// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Mako.Global.Exception;
using Mako.Model;
using Mako.Net;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal partial class FeedEngine(MakoClient makoClient, EngineHandle? engineHandle) : AbstractPixivFetchEngine<Feed>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<Feed> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new UserFeedsAsyncEnumerator(this);

    private partial class UserFeedsAsyncEnumerator(FeedEngine pixivFetchEngine)
        : AbstractPixivAsyncEnumerator<Feed, string, FeedEngine>(pixivFetchEngine, MakoApiKind.WebApi)
    {
        private FeedRequestContext? _feedRequestContext;
        private string? _tt;

        public override async ValueTask<bool> MoveNextAsync()
        {
            if (IsCancellationRequested || PixivFetchEngine.EngineHandle.IsCompleted)
                return false;

            if (_feedRequestContext is null)
            {
                if (await GetResponseAsync(RequestUrl).ConfigureAwait(false) is { } response)
                {
                    if (TryParsePreloadJsonFromHtml(response, out var result))
                    {
                        await UpdateAsync(result).ConfigureAwait(false);
                        _tt = TtRegex.Match(response).Groups["tt"].Value;
                        _feedRequestContext = ExtractRequestContextFromHtml(response);
                    }
                    else
                        return false;
                }
                else
                    return false;
            }

            if (CurrentEntityEnumerator!.MoveNext())
                return true;

            if (_feedRequestContext!.IsLastPage)
            {
                PixivFetchEngine.EngineHandle.Complete();
                return false;
            }

            if (await GetResponseAsync(RequestUrl).ConfigureAwait(false) is { } json) // Else request a new page
            {
                if (IsCancellationRequested)
                    return false;

                await UpdateAsync(ParseFeedJson(JsonDocument.Parse(json).RootElement.GetProperty("stacc"))).ConfigureAwait(false);
                _feedRequestContext = ExtractRequestContextFromJsonElement(JsonDocument.Parse(json).RootElement.GetProperty("stacc"));
                return true;
            }

            return false;
        }

        private static FeedRequestContext? ExtractRequestContextFromHtml(string html)
        {
            if (TryExtractPreloadJson(html, out var json))
            {
                try
                {
                    return ExtractRequestContextFromJsonElement(JsonDocument.Parse(json).RootElement);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        private static FeedRequestContext? ExtractRequestContextFromJsonElement(JsonElement stacc)
        {
            var mode = stacc.GetProperty("param").GetPropertyString("mode");
            var unifyToken = stacc.GetProperty("param").GetPropertyString("unify_token");
            var sid = stacc.GetPropertyString("next_max_sid");
            var isLastPage = stacc.GetPropertyLong("is_last_page") == 1;
            return unifyToken is null || sid is null || mode is null ? null : new FeedRequestContext(unifyToken, sid, mode, isLastPage);
        }

        private static bool TryParsePreloadJsonFromHtml(string html, out IEnumerable<Task<Feed?>> result)
        {
            if (TryExtractPreloadJson(html, out var json))
            {
                result = ParseFeedJson(JsonDocument.Parse(json).RootElement);
                return true;
            }

            result = [];
            return false;
        }

        private static bool TryExtractPreloadJson(string html, out string json)
        {
            var match = PreloadRegex.Match(html);
            if (match.Success)
            {
                json = match.Groups["json"].Value;
                return true;
            }

            json = string.Empty;
            return false;
        }

        private static IEnumerable<Task<Feed?>> ParseFeedJson(JsonElement stacc)
        {
            var users = stacc.GetPropertyOrNull("user").EnumerateObjectOrEmpty();
            var illusts = stacc.GetPropertyOrNull("illust").EnumerateObjectOrEmpty();
            var novels = stacc.GetPropertyOrNull("novel").EnumerateObjectOrEmpty();
            var statuses = stacc.GetPropertyOrNull("status").EnumerateObjectOrEmpty();
            var timelines = stacc.GetPropertyOrNull("timeline").EnumerateObjectOrEmpty();

            return timelines.SelectNotNull(timeline => Task.Run(() => ParseFeed(timeline)));

            Feed? ParseFeed(JsonProperty timeline)
            {
                var id = timeline.Name;
                var status = statuses.FirstOrNull(st => st.Name == id);
                if (!status.HasValue)
                {
                    return null;
                }

                FeedType? feedType = status.Value.GetPropertyString("type") switch
                {
                    "add_bookmark" => FeedType.AddBookmark,
                    "add_illust" => FeedType.PostIllust,
                    "add_novel_bookmark" => FeedType.AddNovelBookmark,
                    "add_favorite" => FeedType.AddFavorite,
                    _ => null
                };

                var feedTargetId = feedType switch
                {
                    FeedType.AddBookmark or FeedType.PostIllust => status.Value.GetProperty("ref_illust").GetPropertyString("id"),
                    FeedType.AddFavorite => status.Value.GetProperty("ref_user").GetPropertyLong("id").ToString(), // long & string in two objects with almost the same properties? fuck pixiv
                    FeedType.AddNovelBookmark => status.Value.GetProperty("ref_novel").GetPropertyString("id"),
                    _ => null
                };

                if (feedTargetId is null)
                {
                    return null; // a feed with null target ID is considered useless because we cannot track its target
                }

                var feedTargetThumbnail = feedType switch
                {
                    FeedType.AddBookmark or FeedType.PostIllust => illusts.FirstOrNull(i => i.Name == feedTargetId)
                        ?.GetPropertyOrNull("url")
                        ?.GetPropertyOrNull("m")
                        ?.GetString(),
                    FeedType.AddFavorite => users.FirstOrNull(u => u.Name == feedTargetId)
                        ?.GetPropertyOrNull("profile_image")
                        .EnumerateObjectOrEmpty()
                        .FirstOrNull()
                        ?.GetPropertyOrNull("url")
                        ?.GetPropertyOrNull("m")
                        ?.GetString(),
                    FeedType.AddNovelBookmark => novels.FirstOrNull(n => n.Name == feedTargetId)
                        ?.GetPropertyOrNull("url")
                        ?.GetPropertyOrNull("m")
                        ?.GetString(),
                    _ => null
                };

                var postDate = DateTime.ParseExact(status.Value.GetPropertyString("post_date")!, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                var postUserId = status.Value.GetProperty("post_user").GetPropertyLong("id").ToString();
                var postUser = users.FirstOrNull(u => u.Name == postUserId);

                if (!postUser.HasValue)
                {
                    return null;
                }

                var postUserName = postUser.Value.GetPropertyString("name");
                var postUserThumbnail = postUser.Value
                    .GetPropertyOrNull("profile_image")
                    .EnumerateObjectOrEmpty()
                    .FirstOrNull()
                    ?.GetPropertyOrNull("url")
                    ?.GetPropertyOrNull("m")
                    ?.GetString();

                if (!long.TryParse(feedTargetId, out var feedIdLong))
                {
                    return null;
                }

                var feedObject = new Feed
                {
                    Id = feedIdLong,
                    FeedThumbnail = feedTargetThumbnail,
                    Type = feedType,
                    PostDate = postDate,
                    PostUserId = postUserId,
                    PostUsername = postUserName,
                    PostUserThumbnail = postUserThumbnail
                };

                switch (feedType)
                {
                    case FeedType.AddBookmark or FeedType.PostIllust:
                    {
                        var illustration = illusts.FirstOrNull(i => i.Name == feedTargetId);
                        feedObject.ArtistName = users.FirstOrNull(u => u.Name == (illustration?.GetPropertyOrNull("post_user")?.GetPropertyOrNull("id")?.GetString() ?? string.Empty))?.GetPropertyOrNull("name")?.GetString();
                        feedObject.FeedName = illustration?.GetPropertyOrNull("title")?.GetString();
                        break;
                    }
                    case FeedType.AddFavorite:
                        feedObject.FeedName = users.FirstOrNull(u => u.Name == feedTargetId)?.GetPropertyOrNull("name")?.GetString();
                        feedObject.IsTargetRefersToUser = true;
                        break;
                }

                return feedObject;
            }
        }

        private async Task UpdateAsync(IEnumerable<Task<Feed?>> result)
        {
            CurrentEntityEnumerator = (await Task.WhenAll(result).ConfigureAwait(false)).WhereNotNull().GetEnumerator();
            PixivFetchEngine.RequestedPages++;
        }

        private string RequestUrl => _feedRequestContext is null
            ? "/stacc?mode=unify"
            : $"/stacc/my/home/all/activity/{_feedRequestContext.Sid}/.json"
              + $"?mode={_feedRequestContext.Mode}"
              + $"&unify_token={_feedRequestContext.UnifyToken}"
              + $"&tt={_tt}";

        private async Task<string?> GetResponseAsync(string url)
        {
            try
            {
                var responseMessage = await ApiClient.GetAsync(url).ConfigureAwait(false);
                if (responseMessage.IsSuccessStatusCode)
                    return await responseMessage.Content.ReadAsStringAsync();

                MakoClient.LogException(new MakoNetworkException(url,
                    MakoClient.Configuration.DomainFronting,
                    await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false),
                    (int) responseMessage.StatusCode));
                return null;
            }
            catch (Exception e)
            {
                MakoClient.LogException(new MakoNetworkException(url, MakoClient.Configuration.DomainFronting, e.Message, (int?) (e as HttpRequestException)?.StatusCode ?? -1));
                return null;
            }
        }

        [GeneratedRegex("tt: \"(?<tt>.*)\"")]
        private static partial Regex TtRegex { get; }

        [GeneratedRegex(@"pixiv\.stacc\.env\.preload\.stacc \= (?<json>.*);")]
        private static partial Regex PreloadRegex { get; }
    }

    /// <summary>
    /// Required parameters established from multiple tests, I don't know what do they mean
    /// </summary>
    private record FeedRequestContext(string UnifyToken, string Sid, string Mode, bool IsLastPage);
}
