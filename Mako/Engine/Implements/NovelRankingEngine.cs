// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class NovelRankingEngine(
    MakoClient makoClient,
    RankOption rankOption,
    DateOnly dateOnly,
    TargetFilter targetFilter,
    EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<Novel>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<NovelRankingEngine>(
            this,
            "/v1/novel/ranking" +
            $"?filter={targetFilter.GetDescription()}" +
            $"&mode={rankOption.GetDescription()}" +
            $"&date={dateOnly:yyyy-MM-dd}");
}
