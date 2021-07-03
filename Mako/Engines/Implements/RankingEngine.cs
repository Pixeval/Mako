using System;
using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines.Implements
{
    internal class RankingEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly DateTime _dateTime;
        private readonly RankOption _rankOption;
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
            return RecursivePixivAsyncEnumerators.Illustration<RankingEngine>.WithInitialUrl(this, MakoApiKind.AppApi,
                engine => $"/v1/illust/ranking?filter={engine._targetFilter.GetDescription()}&mode={engine._rankOption.GetDescription()}&date={engine._dateTime:yyyy-MM-dd}")!;
        }
    }
}