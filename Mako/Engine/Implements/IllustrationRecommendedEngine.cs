// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

[method: MakoExtensionConstructor]
internal class IllustrationRecommendedEngine(
    MakoClient makoClient,
    bool includeRankingIllusts = true,
    bool includePrivacyPolicy = true,
    // 下面三个参数好像没用
    WorkType? contentType = null,
    uint? maxBookmarkIdForRecommend = null,
    uint? minBookmarkIdForRecentIllustration = null)
    : AbstractPixivFetchEngine<Illustration>(makoClient)
{
    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<IllustrationRecommendedEngine>(this,
            $"/v1/illust/recommended"
            + $"?{TargetFilterParam}"
            + $"&include_ranking_illusts={includeRankingIllusts.ToString().ToLower()}"
            + $"&include_privacy_policy={includePrivacyPolicy.ToString().ToLower()}"
            + contentType?.Let(static s => $"&content_type={s.GetDescription()}")
            + maxBookmarkIdForRecommend?.Let(static s => $"&max_bookmark_id_for_recommend={s}")
            + minBookmarkIdForRecentIllustration?.Let(static s => $"&min_bookmark_id_for_recent_illust={s}"));
}
