// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class IllustrationRankingEngine : AbstractPixivFetchEngine<Illustration>
{
    private readonly RankOption _rankOption;
    private readonly DateOnly _dateOnly;

    /// <summary>
    /// Request ranking in Pixiv.
    /// </summary>
    /// <param name="makoClient"></param>
    /// <param name="rankOption">The option of which the <see cref="RankOption" /> of rankings</param>
    /// <param name="dateTime">The date of rankings</param>
    /// <returns>
    /// The <see cref="IFetchEngine{T}" /> containing rankings.
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MakoExtensionConstructor]
    public IllustrationRankingEngine(
        MakoClient makoClient,
        RankOption rankOption,
        DateTimeOffset dateTime) : base(makoClient)
    {
        if (!RankOption.IsIllustrationSupport(rankOption))
            throw new ArgumentOutOfRangeException(nameof(rankOption));
        var dateOnly = dateTime.ToJapanTime().ToDateOnly();
        MakoClient.CheckRankingMaxDate(dateOnly);

        _rankOption = rankOption;
        _dateOnly = dateOnly;
    }

    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<IllustrationRankingEngine>(
            this,
            $"/v1/illust/ranking" +
            $"?{TargetFilterParam}" +
            $"&mode={_rankOption.GetDescription()}" +
            $"&date={_dateOnly:yyyy-MM-dd}");
}
