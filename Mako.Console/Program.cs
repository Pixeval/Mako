using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Mako.Model;
using Mako.Util;

namespace Mako.Console
{
    public static class Program
    {
        private static readonly Session Session = ""
            .FromJson<TokenResponse>(option => option.IgnoreNullValues = true)!
            .ToSession("123456")
            .UseBypass()
            .UseCache();

        private static readonly MakoClient MakoClient = new(Session, CultureInfo.CurrentCulture);

        private static async Task GetBookmark()
        {
            var bookmarks = MakoClient.Bookmarks("7263576", PrivacyPolicy.Public);
            await foreach (var i in bookmarks)
            {
                if (i != null)
                {
                    System.Console.WriteLine(i.Id);
                }
            }
        }

        private static async Task Search()
        {
            var cnt = 0;
            var search = MakoClient.Search("東方project", pages: 6, sortOption: IllustrationSortOption.Popularity);
            await foreach (var i in search)
            {
                cnt++;
                if (i != null)
                {
                    System.Console.WriteLine(i.Id);
                }
            }

            System.Console.WriteLine(cnt);
        }
        
        public static async Task Main()
        {
            await Search();
        }
    }
}