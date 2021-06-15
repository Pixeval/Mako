using System.Collections.Generic;
using Mako.Model;

namespace Mako.Util
{
    internal class IllustrationPopularityComparator : IComparer<Illustration>
    {
        public int Compare(Illustration? x, Illustration? y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return x.Bookmarks < y.Bookmarks ? 1 : x.Bookmarks == y.Bookmarks ? 0 : -1;
        }
    }

    internal class IllustrationPublishDateComparator : IComparer<Illustration>
    {
        public int Compare(Illustration? x, Illustration? y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return x.PublishDate < y.PublishDate ? 1 : x.PublishDate == y.PublishDate ? 0 : -1;
        }
    }
}