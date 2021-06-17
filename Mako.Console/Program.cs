using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Util;

namespace Mako.Console
{
    [PublicAPI]
    public static class Program
    {
        private static readonly Session Session = ""
            .FromJson<TokenResponse>(option => option.IgnoreNullValues = true)!
            .ToSession("123456")
            .UseBypass()
            .UseCache();

        private static readonly MakoClient MakoClient = new(Session, CultureInfo.CurrentCulture);

        private static async Task PrintIllusts(IAsyncEnumerable<Illustration> illustrations)
        {
            await foreach (var i in illustrations)
            {
                if (i != null)
                {
                    System.Console.WriteLine(i.Id);
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
        
        public static async Task Main()
        {
            await Recommend();
        }
    }
}