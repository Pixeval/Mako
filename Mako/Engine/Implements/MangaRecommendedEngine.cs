// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

[method: MakoExtensionConstructor]
internal class MangaRecommendedEngine(
    MakoClient makoClient,
    bool includeRankingIllusts = true,
    bool includePrivacyPolicy = true)
    : AbstractPixivFetchEngine<Illustration>(makoClient)
{
    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<MangaRecommendedEngine>(this,
            $"/v1/illust/recommended"
            + $"?{TargetFilterParam}"
            + $"&include_ranking_illusts={includeRankingIllusts.ToString().ToLower()}"
            + $"&include_privacy_policy={includePrivacyPolicy.ToString().ToLower()}");
}
