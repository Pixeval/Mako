// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class RecommendNovelEngine(
    MakoClient makoClient,
    TargetFilter filter,
    uint? maxBookmarkIdForRecommend,
    EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<Novel>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<RecommendNovelEngine>(this,
            "/v1/novel/recommended"
            + $"?filter={filter.GetDescription()}"
            + maxBookmarkIdForRecommend?.Let(static s => $"&max_bookmark_id_for_recommend={s}"));
}
