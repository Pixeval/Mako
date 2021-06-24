using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Mako.Model;
using Mako.Util;

namespace Mako.Console
{
    public class FeedJsonTest
    {
        public static Task<Feed?>[] ParsePreloadJson(string str)
        {
            var root = JsonDocument.Parse(str).RootElement;
            var users = root.GetProperty("user").EnumerateObject();
            var illusts = root.GetProperty("illust").EnumerateObject();
            var novels = root.GetProperty("novel").EnumerateObject();
            var statuses = root.GetProperty("status").EnumerateObject();
            var timelines = root.GetProperty("timeline").EnumerateObject();

            return timelines.SelectNotNull(timeline => Task.Run(() => ParseFeed(timeline))).ToArray();

            Feed? ParseFeed(JsonProperty timeline)
            {
                var id = timeline.Name;
                var status = statuses.First(st => st.Name == id);
                FeedType? feedType = status.GetPropertyString("type") switch
                {
                    "add_bookmark"       => FeedType.AddBookmark,
                    "add_illust"         => FeedType.AddIllust,
                    "add_novel_bookmark" => FeedType.AddNovelBookmark,
                    "add_favorite"       => FeedType.AddFavorite,
                    _                    => null
                };
                var feedTargetId = feedType switch
                {
                    FeedType.AddBookmark or FeedType.AddIllust => status.GetProperty("ref_illust").GetPropertyString("id"),
                    FeedType.AddFavorite                       => status.GetProperty("ref_user").GetPropertyLong("id").ToString(), // long & string in two objects with almost the same properties? fuck pixiv
                    FeedType.AddNovelBookmark                  => status.GetProperty("ref_novel").GetPropertyString("id"),
                    _                                          => null
                };
                if (feedTargetId is null)
                {
                    return null; // a feed with null target ID is considered useless because we cannot track its target
                }

                var feedTargetThumbnail = feedType switch
                {
                    FeedType.AddBookmark or FeedType.AddIllust => illusts.First(i => i.Name == feedTargetId).GetProperty("url").GetPropertyString("m"),
                    FeedType.AddFavorite                       => users.First(u => u.Name == feedTargetId).GetProperty("profile_image").GetProperty("url").GetPropertyString("m"),
                    FeedType.AddNovelBookmark                  => novels.First(n => n.Name == feedTargetId).Value.GetProperty("url").GetPropertyString("m"),
                    _                                          => null
                };
                var postDate = DateTime.ParseExact(status.GetPropertyString("post_date")!, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                var postUserId = status.GetProperty("post_user").GetPropertyLong("id").ToString();
                var postUser = users.First(u => u.Name == postUserId);
                var postUserName = postUser.GetPropertyString("name");
                var postUserThumbnail = postUser.GetProperty("profile_image").GetProperty("url").GetPropertyString("m");
                var feedObject = new Feed
                {
                    FeedId = feedTargetId,
                    FeedThumbnail = feedTargetThumbnail,
                    Type = feedType,
                    PostDate = postDate,
                    PostUserId = postUserId,
                    PostUserName = postUserName,
                    PostUserThumbnail = postUserThumbnail
                };

                switch (feedType)
                {
                    case FeedType.AddBookmark or FeedType.AddIllust:
                    {
                        var illustration = illusts.First(i => i.Name == feedTargetId);
                        feedObject.ArtistName = users.First(u => u.Name == illustration.GetProperty("post_user").GetPropertyString("id")).GetPropertyString("name");
                        feedObject.FeedName = illustration.GetPropertyString("title");
                        break;
                    }
                    case FeedType.AddFavorite:
                        feedObject.FeedName = users.First(u => u.Name == feedTargetId).GetPropertyString("name");
                        feedObject.IsTargetRefersToUser = true;
                        break;
                }

                return feedObject;
            }
        }
    }
}