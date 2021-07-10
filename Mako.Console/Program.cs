#region Copyright (c) Pixeval/Mako

// MIT License
// 
// Copyright (c) Pixeval 2021 Mako.Console/Program.cs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Preference;
using Mako.Util;

namespace Mako.Console
{
    /// <summary>
    ///     由于登录原因测试没法自动化，干脆这样好了
    /// </summary>
    [PublicAPI]
    public static class Program
    {
        private static readonly Session Session = "{\"Name\":\"December0730\",\"ExpireIn\":\"2021-07-04T09:32:20.2335628+08:00\",\"AccessToken\":\"6VFY3ocrUl8N9eNoUimbQjvKEYhJ0SKln8YtEuE7MlY\",\"RefreshToken\":\"5QOHeUpB-cFNjrXmqOU0YFv2QLnlnKHmJ4RJF4YSSQI\",\"AvatarUrl\":\"https://i.pximg.net/user-profile/img/2019/01/13/21/00/08/15255001_2f78dcb00cc01551c55586280352571a_170.jpg\",\"Id\":\"17861677\",\"Account\":\"2653221698\",\"IsPremium\":false,\"Cookie\":\"__cf_bm=a469cca0ba4d4dc5cb55e3aae5535c3c37ef52cd-1625359041-1800-AZX9Yibv4JCJKB5EUVwuPeXdLI1Vt3S/4lL9f8SHQ8yf2jdW\\u002BxHZP3SP\\u002B7mvDFJcKcC\\u002BAt17Qf3vb4Ma05HKoiABF/SuCMOvO\\u002BjsWH0fKDz2F0l56CN2GtDWa6GmhIJ6SARLa/W\\u002BI3yQ4RfRkKEja/Awr1q\\u002B2nJ6Tvabcd9ViPgKJugSZLUeNbjK6gc2djZNVg==;privacy_policy_agreement=3;p_ab_id=5;d_type=1;b_type=1;device_token=a799b12b2bae4cd02e715c273314030d;_gat=1;c_type=21;PHPSESSID=17861677_JQgKd8M21Nkbp82KCrBnsdbPK4Hxnvni;p_ab_id_2=8;_gid=GA1.2.1677177896.1625284371;privacy_policy_notification=0;_ga=GA1.2.1567916307.1623632304;a_type=0;p_ab_d_id=626740158;\"}"
            .FromJson<Session>()!;

        private static readonly MakoClient MakoClient = new(Session, new MakoClientConfiguration
        {
            Bypass = true,
            CultureInfo = new CultureInfo("zh-cn")
        });

        private static readonly Action<JsonSerializerOptions> DefaultSerializerOptions = options =>
        {
            options.WriteIndented = true;
            options.IgnoreNullValues = true;
            options.ReferenceHandler = ReferenceHandler.Preserve;
        };

        static Program()
        {
            System.Console.OutputEncoding = Encoding.UTF8;
        }

        private static void PrintTrendingTags(IEnumerable<TrendingTag> tags)
        {
            var cnt = 0;
            foreach (var i in tags)
            {
                if (i != null)
                {
                    cnt++;
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }

            System.Console.WriteLine($"Count: {cnt}");
        }

        private static async Task PrintIllusts(IAsyncEnumerable<Illustration> illustrations)
        {
            var cnt = 0;
            await foreach (var i in illustrations)
            {
                if (i != null)
                {
                    cnt++;
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }

            System.Console.WriteLine($"Count: {cnt}");
        }

        private static async Task PrintNovels(IAsyncEnumerable<Novel> novels)
        {
            var cnt = 0;
            await foreach (var i in novels)
            {
                if (i != null)
                {
                    cnt++;
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }

            System.Console.WriteLine($"Count: {cnt}");
        }

        private static async Task PrintUser(IAsyncEnumerable<User> users)
        {
            var cnt = 0;
            await foreach (var i in users)
            {
                if (i != null)
                {
                    cnt++;
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }

            System.Console.WriteLine($"Count: {cnt}");
        }

        private static async Task PrintSpotlight(IAsyncEnumerable<SpotlightArticle> articles)
        {
            var cnt = 0;
            await foreach (var i in articles)
            {
                if (i != null)
                {
                    cnt++;
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }

            System.Console.WriteLine($"Count: {cnt}");
        }

        private static async Task PrintFeeds(IAsyncEnumerable<Feed> feeds)
        {
            var cnt = 0;
            await foreach (var i in feeds)
            {
                if (i != null)
                {
                    cnt++;
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }

            System.Console.WriteLine($"Count: {cnt}");
        }

        private static async Task Recommend()
        {
            var recommend = MakoClient.Recommends(RecommendContentType.Manga);
            await PrintIllusts(recommend);
        }

        private static async Task Ranking()
        {
            var ranking = MakoClient.Ranking(RankOption.Day, DateTime.Today - TimeSpan.FromDays(2));
            await PrintIllusts(ranking);
        }

        private static async Task GetBookmark()
        {
            var bookmarks = MakoClient.Bookmarks("7263576", PrivacyPolicy.Public);
            await PrintIllusts(bookmarks);
        }

        private static async Task Search()
        {
            var search = MakoClient.Search("東方project", pages: 6, sortOption: IllustrationSortOption.PopularityDescending);
            await PrintIllusts(search);
        }

        private static async Task RecommendIllustrators()
        {
            var illustrators = MakoClient.RecommendIllustrators();
            await PrintUser(illustrators);
        }

        private static async Task Spotlights()
        {
            var spotlights = MakoClient.Spotlights();
            await PrintSpotlight(spotlights);
        }

        private static async Task Feeds()
        {
            var feeds = MakoClient.Feeds();
            await PrintFeeds(feeds);
        }

        private static async Task Uploads()
        {
            var uploads = MakoClient.Posts("333556");
            await PrintIllusts(uploads);
        }

        private static async Task Following()
        {
            var follows = MakoClient.Following("333556", PrivacyPolicy.Public);
            await PrintUser(follows);
        }

        private static async Task SearchUser()
        {
            var users = MakoClient.SearchUser("ideolo");
            await PrintUser(users);
        }

        private static async Task Updates()
        {
            var updates = MakoClient.RecentPosts(PrivacyPolicy.Public);
            await PrintIllusts(updates);
        }

        private static async Task GetSpotlightIllustrations(string id)
        {
            var spotlightDetail = await MakoClient.GetSpotlightDetailAsync(id);
            await PrintIllusts(Enumerates.ToAsyncEnumerable(spotlightDetail!.Illustrations));
        }

        private static async Task TrendingTags()
        {
            var tags = await MakoClient.GetTrendingTagsAsync(TargetFilter.ForAndroid);
            PrintTrendingTags(tags);
        }

        private static async Task UserTaggedBookmarksId()
        {
            var tags = await MakoClient.GetUserSpecifiedBookmarkTagsAsync("333556");
            if (!tags.Any())
            {
                System.Console.WriteLine("Empty tags!");
            }

            var (tag, _) = tags.Aggregate(tags.First(), (lhs, rhs) => lhs.Key.Count > rhs.Key.Count ? lhs : rhs);
            var ids = MakoClient.UserTaggedBookmarksId("333556", tag.Tag.Name!).Distinct();
            var cnt = 0;
            await foreach (var id in ids)
            {
                System.Console.WriteLine(id);
                cnt++;
            }

            System.Console.WriteLine($"Count: {cnt}");
        }

        private static async Task UserTaggedBookmarks()
        {
            var tags = await MakoClient.GetUserSpecifiedBookmarkTagsAsync("333556");
            if (!tags.Any())
            {
                System.Console.WriteLine("Empty tags!");
            }

            var (tag, _) = tags.ToList()[new Random().Next(0, tags.Count)];
            var ids = MakoClient.UserTaggedBookmarks("333556", tag.Tag.Name!).Distinct();
            await PrintIllusts(ids);
        }

        private static async Task NovelPosts()
        {
            var novels = MakoClient.NovelPosts("11", TargetFilter.ForAndroid);
            await PrintNovels(novels);
        }

        private static async Task MangaPosts()
        {
            var mangas = MakoClient.MangaPosts("69239259", TargetFilter.ForAndroid);
            await PrintIllusts(mangas);
        }

        private static async Task NovelBookmarks()
        {
            var novels = MakoClient.NovelBookmarks(Session.Id!, PrivacyPolicy.Private, TargetFilter.ForAndroid);
            await PrintNovels(novels);
        }

        public static async Task Main()
        {
        }
    }
}