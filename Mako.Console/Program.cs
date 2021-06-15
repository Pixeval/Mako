using System;
using System.Collections.Generic;
using System.Globalization;
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
            .Apply(s => s.Bypass = true);

        private static readonly MakoClient MakoClient = new(Session, CultureInfo.CurrentCulture);

        public static async Task Main(string[] args)
        {
            var bookmarks = MakoClient.Bookmarks("333556", PrivacyPolicy.Public);
            await foreach (var i in bookmarks)
            {
                if (i != null)
                {
                    System.Console.WriteLine(i.Id);
                }
            }
        }
    }
}