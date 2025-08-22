// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class IllustrationRankingEngine(
    MakoClient makoClient,
    RankOption rankOption,
    DateOnly dateOnly,
    TargetFilter targetFilter,
    EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<Illustration>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<IllustrationRankingEngine>(
            this,
            $"/v1/illust/ranking" +
            $"?filter={targetFilter.GetDescription()}" +
            $"&mode={rankOption.GetDescription()}" +
            $"&date={dateOnly:yyyy-MM-dd}");
}
