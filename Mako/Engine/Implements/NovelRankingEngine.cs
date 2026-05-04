// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class NovelRankingEngine : AbstractPixivFetchEngine<Novel>
{
    private readonly RankOption _rankOption;
    private readonly DateOnly _dateOnly;

    /// <inheritdoc cref="IllustrationRankingEngine.IllustrationRankingEngine" />
    [MakoExtensionConstructor]
    public NovelRankingEngine(
        MakoClient makoClient,
        RankOption rankOption,
        DateTimeOffset dateTime) : base(makoClient)
    {
        if (!RankOption.IsNovelSupport(rankOption))
            throw new ArgumentOutOfRangeException(nameof(rankOption));
        var dateOnly = dateTime.ToJapanTime().ToDateOnly();
        MakoClient.CheckRankingMaxDate(dateOnly);

        _rankOption = rankOption;
        _dateOnly = dateOnly;
    }

    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<NovelRankingEngine>(
            this,
            "/v1/novel/ranking" +
            $"?{TargetFilterParam}" +
            $"&mode={_rankOption.GetDescription()}" +
            $"&date={_dateOnly:yyyy-MM-dd}");
}
