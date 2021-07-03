using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Mako.Util;

namespace Mako.Engines.Implements
{
    internal class SpotlightArticleEngine : AbstractPixivFetchEngine<SpotlightArticle>
    {
        public SpotlightArticleEngine([NotNull] MakoClient makoClient, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
        }

        public override IAsyncEnumerator<SpotlightArticle> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new SpotlightArticleAsyncEnumerator(this, MakoApiKind.AppApi)!;
        }

        private class SpotlightArticleAsyncEnumerator : RecursivePixivAsyncEnumerator<SpotlightArticle, PixivSpotlightResponse, SpotlightArticleEngine>
        {
            public SpotlightArticleAsyncEnumerator([NotNull] SpotlightArticleEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override bool ValidateResponse(PixivSpotlightResponse rawEntity)
            {
                return rawEntity.SpotlightArticles.IsNotNullOrEmpty();
            }

            protected override string? NextUrl(PixivSpotlightResponse? rawEntity)
            {
                return rawEntity?.NextUrl;
            }

            protected override string InitialUrl()
            {
                return "/v1/spotlight/articles?category=all";
            }

            protected override IEnumerator<SpotlightArticle>? GetNewEnumerator(PixivSpotlightResponse? rawEntity)
            {
                return rawEntity?.SpotlightArticles?.GetEnumerator();
            }
        }
    }
}