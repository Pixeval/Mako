using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Mako.Util;

namespace Mako.Engines.Implements
{
    internal class RecommendEngine : AbstractPixivFetchEngine<Illustration>
    {

        private readonly RecommendContentType _recommendContentType;
        private readonly TargetFilter _filter;
        private readonly uint? _maxBookmarkIdForRecommend;
        private readonly uint? _minBookmarkIdForRecentIllust;

        public RecommendEngine(MakoClient makoClient, RecommendContentType? recommendContentType, TargetFilter filter, uint? maxBookmarkIdForRecommend, uint? minBookmarkIdForRecentIllust, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _recommendContentType = recommendContentType ?? RecommendContentType.Illust;
            _filter = filter;
            _maxBookmarkIdForRecommend = maxBookmarkIdForRecommend;
            _minBookmarkIdForRecentIllust = minBookmarkIdForRecentIllust;
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new RecommendAsyncEnumerator(this, MakoApiKind.AppApi)!;
        }

        private class RecommendAsyncEnumerator : RecursivePixivAsyncEnumerator<Illustration, PixivResponse, RecommendEngine>
        {
            public RecommendAsyncEnumerator([NotNull] RecommendEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override bool ValidateResponse(PixivResponse rawEntity)
            {
                return rawEntity.Illusts.IsNotNullOrEmpty();
            }

            protected override string? NextUrl(PixivResponse? rawEntity) => rawEntity?.NextUrl;

            protected override string InitialUrl()
            {
                var maxBookmarkIdForRecommend = PixivFetchEngine._maxBookmarkIdForRecommend?.Let(static s => $"&max_bookmark_id_for_recommend={s}") ?? string.Empty;
                var maxBookmarkIdForRecentIllust = PixivFetchEngine._minBookmarkIdForRecentIllust.Let(static s => $"&min_bookmark_id_for_recent_illust={s}") ?? string.Empty;
                return $"/v1/illust/recommended?filter={PixivFetchEngine._filter.GetDescription()}&content_type={PixivFetchEngine._recommendContentType.GetDescription()}{maxBookmarkIdForRecommend}{maxBookmarkIdForRecentIllust}";
            }

            protected override IEnumerator<Illustration>? GetNewEnumerator(PixivResponse? rawEntity)
            {
                return rawEntity?.Illusts?.SelectNotNull(MakoExtension.ToIllustration).GetEnumerator();
            }
        }
    }
}