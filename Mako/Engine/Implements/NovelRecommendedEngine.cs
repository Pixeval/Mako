// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <inheritdoc cref="NovelRecommendedEngine.NovelRecommendedEngine" />
[method: MakoExtensionConstructor(true)]
internal class NovelRecommendedEngine(
    MakoClient makoClient,
    bool includeRankingNovels = true,
    bool includePrivacyPolicy = true,
    // 下面这个参数好像没用
    uint? maxBookmarkIdForRecommend = null)
    : AbstractPixivFetchEngine<Novel>(makoClient)
{
    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<NovelRecommendedEngine>(this,
            "/v1/novel/recommended"
            + $"?{TargetFilterParam}"
            + $"&include_ranking_novels={includeRankingNovels.ToString().ToLower()}"
            + $"&include_privacy_policy={includePrivacyPolicy.ToString().ToLower()}"
            + maxBookmarkIdForRecommend?.Let(static s => $"&max_bookmark_id_for_recommend={s}"));
}
