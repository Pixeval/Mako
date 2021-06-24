using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            await foreach (var i in illustrations)
            {
                if (i != null)
                {
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }
        }
        
        private static async Task PrintUser(IAsyncEnumerable<User> users)
        {
            await foreach (var i in users)
            {
                if (i != null)
                {
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }
        }
        
        private static async Task PrintSpotlight(IAsyncEnumerable<SpotlightArticle> articles)
        {
            await foreach (var i in articles)
            {
                if (i != null)
                {
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }
        }

        private static async Task PrintFeeds(IAsyncEnumerable<Feed> feeds)
        {
            await foreach (var i in feeds)
            {
                if (i != null)
                {
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }
        }

        private static async Task Recommend()
        {
            var recommend = MakoClient.Recommends();
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
            var search = MakoClient.Search("東方project", pages: 6, sortOption: IllustrationSortOption.Popularity);
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
            var uploads = MakoClient.Uploads("333556");
            await PrintIllusts(uploads);
        }
        
        public static async Task Main()
        {
            await Uploads();
        }
    }
}