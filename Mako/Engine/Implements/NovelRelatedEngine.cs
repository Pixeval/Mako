// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <inheritdoc cref="IllustrationFollowingEngine.IllustrationFollowingEngine" />
[method: MakoExtensionConstructor(true)]
internal class NovelRelatedEngine(MakoClient makoClient, long novelId)
    : AbstractPixivFetchEngine<Novel>(makoClient)
{
    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<NovelRelatedEngine>(
            this,
            "/v1/novel/related"
            // pixiv 安卓这里用的是POST url-encoded, 但这样GET也行，估计是笔误
            + $"?novel_id={novelId}");
}
