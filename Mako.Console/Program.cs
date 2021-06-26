using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Util;

namespace Mako.Console
{
    /// <summary>
    /// 由于登录原因测试没法自动化，干脆这样好了
    /// </summary>
    [PublicAPI]
    public static class Program
    {
        private static readonly Session Session = ""
            .FromJson<Session>()!
            .UseBypass()
            .UseCache();

        private static readonly MakoClient MakoClient = new(Session, CultureInfo.CurrentCulture);

        private static readonly Action<JsonSerializerOptions> DefaultSerializerOptions = options =>
        {
            options.WriteIndented = true;
            options.IgnoreNullValues = true;
            options.ReferenceHandler = ReferenceHandler.Preserve;
        };
        
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
            var illustrators = MakoClient.RecommendIllustratorsIncomplete();
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
            var uploads = MakoClient.Uploads("333556");
            await PrintIllusts(uploads);
        }

        private static async Task Following()
        {
            var follows = MakoClient.FollowingIncomplete("333556", PrivacyPolicy.Private);
            await PrintUser(follows);
        }

        private static async Task SearchUser()
        {
            var users = MakoClient.SearchUserIncomplete("ideolo");
            await PrintUser(users);
        }
        
        private static async Task Updates()
        {
            var updates = MakoClient.Updates(PrivacyPolicy.Public);
            await PrintIllusts(updates);
        }

        
        public static async Task Main()
        {
            
        }
    }
}