// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class RecommendedIllustrationEngine(
    MakoClient makoClient,
    WorkType? contentType,
    uint? maxBookmarkIdForRecommend,
    uint? minBookmarkIdForRecentIllustration,
    TargetFilter filter,
    EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<Illustration>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<RecommendedIllustrationEngine>(this,
            "/v1/illust/recommended"
            + $"?filter={filter.GetDescription()}"
            + contentType?.Let(static s => $"&content_type={s.GetDescription()}")
            + maxBookmarkIdForRecommend?.Let(static s => $"&max_bookmark_id_for_recommend={s}")
            + minBookmarkIdForRecentIllustration?.Let(static s => $"&min_bookmark_id_for_recent_illust={s}"));
}
