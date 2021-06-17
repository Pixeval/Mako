using System;

namespace Mako.Engines
{
    internal static class Caches
    {
        public static string CreateBookmarkCacheKey(string uid) => uid;

        public static string CreateRankingCacheKey(RankOption rankOption, DateTime dateTime) => $"{rankOption}{dateTime:yyyy-MM-dd}";
    }
}