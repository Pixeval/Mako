// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <inheritdoc cref="IllustrationNewEngine.IllustrationNewEngine" />
[method: MakoExtensionConstructor(true)]
internal class NovelNewEngine(
    MakoClient makoClient,
    uint? maxNovelId)
    : AbstractPixivFetchEngine<Novel>(makoClient)
{
    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<NovelNewEngine>(this,
            "/v1/novel/new"
            + $"?{TargetFilterParam}"
            + maxNovelId?.Let(static s => $"&max_novel_id={s}"));
}
