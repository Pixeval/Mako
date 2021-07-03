using System.Collections.Generic;
using System.Threading;
using System.Web;
using JetBrains.Annotations;
using Mako.Net;
using Mako.Net.Response;
using Mako.Util;

namespace Mako.Engines.Implements
{
    /// <summary>
    ///     Get the bookmarks that have user-defined tags associate with them, only returns their ID in string representation
    ///     This API is not supposed to have other usages
    /// </summary>
    internal class TaggedBookmarksIdEngine : AbstractPixivFetchEngine<string>
    {
        private readonly string _tag;
        private readonly string _uid;

        public TaggedBookmarksIdEngine([NotNull] MakoClient makoClient, EngineHandle? engineHandle, string uid, string tag) : base(makoClient, engineHandle)
        {
            _uid = uid;
            _tag = tag;
        }

        public override IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new TaggedBookmarksIdAsyncEnumerator(this, MakoApiKind.WebApi)!;
        }

        private class TaggedBookmarksIdAsyncEnumerator : RecursivePixivAsyncEnumerator<string, WebApiBookmarksWithTagResponse, TaggedBookmarksIdEngine>
        {
            private int _currentIndex;

            public TaggedBookmarksIdAsyncEnumerator([NotNull] TaggedBookmarksIdEngine pixivFetchEngine, MakoApiKind apiKind) : base(pixivFetchEngine, apiKind)
            {
            }

            protected override bool ValidateResponse(WebApiBookmarksWithTagResponse rawEntity)
            {
                return rawEntity.ResponseBody?.Works.IsNotNullOrEmpty() ?? false;
            }

            protected override string NextUrl(WebApiBookmarksWithTagResponse? rawEntity)
            {
                return GetUrl();
            }

            protected override string InitialUrl()
            {
                return GetUrl();
            }

            protected override IEnumerator<string>? GetNewEnumerator(WebApiBookmarksWithTagResponse? rawEntity)
            {
                _currentIndex++; // Cannot put it in the GetUrl() because the NextUrl() gonna be called twice at each iteration which will increases the _currentIndex by 2
                return rawEntity?.ResponseBody?.Works?.SelectNotNull(w => w.Id, w => w.Id!).GetEnumerator();
            }

            private string GetUrl()
            {
                return $"/ajax/user/{PixivFetchEngine._uid}/illusts/bookmarks?tag={HttpUtility.UrlEncode(PixivFetchEngine._tag)}&offset={_currentIndex * 100}&limit=100&rest=show&lang=";
            }
        }
    }
}