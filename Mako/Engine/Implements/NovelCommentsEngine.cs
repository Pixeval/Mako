// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <inheritdoc cref="IllustrationCommentsEngine.IllustrationCommentsEngine" />
[method: MakoExtensionConstructor]
internal class NovelCommentsEngine(long novelId, MakoClient makoClient)
    : AbstractPixivFetchEngine<Comment>(makoClient)
{
    public override IAsyncEnumerator<Comment> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Comment<NovelCommentsEngine>(
            this,
            "/v3/novel/comments" +
            $"?novel_id={novelId}");
}
