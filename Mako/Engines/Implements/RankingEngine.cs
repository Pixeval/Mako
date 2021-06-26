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
    internal class RankingEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly RankOption _rankOption;
        private readonly DateTime _dateTime;
        private readonly TargetFilter _targetFilter;

        public RankingEngine(
            MakoClient makoClient,
            RankOption rankOption, 
            DateTime dateTime,
            TargetFilter targetFilter,
            EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _rankOption = rankOption;
            _dateTime = dateTime;
            _targetFilter = targetFilter;
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new RankingAsyncEnumerator(this, MakoApiKind.AppApi)!;
        }

        private class RankingAsyncEnumerator : RecursivePixivAsyncEnumerator<Illustration, PixivResponse, RankingEngine>
        {
            public RankingAsyncEnumerator([NotNull] RankingEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override bool ValidateResponse(PixivResponse rawEntity)
            {
                return rawEntity.Illusts.IsNotNullOrEmpty();
            }

            protected override string? NextUrl(PixivResponse? rawEntity) => rawEntity?.NextUrl;

            protected override string InitialUrl() => $"/v1/illust/ranking?filter={PixivFetchEngine._targetFilter.GetDescription()}&mode={PixivFetchEngine._rankOption.GetDescription()}&date={PixivFetchEngine._dateTime:yyyy-MM-dd}";
            
            protected override IEnumerator<Illustration>? GetNewEnumerator(PixivResponse? rawEntity)
            {
                return rawEntity?.Illusts?.SelectNotNull(MakoExtension.ToIllustration).GetEnumerator();
            }
        }
    }
}