using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mako.Model
{
    [PublicAPI]
    public record SpotlightDetail
    {
        public SpotlightDetail(SpotlightArticle spotlightArticle, string introduction, IEnumerable<Illustration> illustrations)
        {
            SpotlightArticle = spotlightArticle;
            Introduction = introduction;
            Illustrations = illustrations;
        }

        public SpotlightArticle SpotlightArticle { get; set; }

        public string Introduction { get; set; }

        public IEnumerable<Illustration> Illustrations { get; set; }
    }
}