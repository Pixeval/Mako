// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <inheritdoc cref="IllustrationCommentRepliesEngine.IllustrationCommentRepliesEngine" />
[method: MakoExtensionConstructor]
internal class NovelCommentRepliesEngine(long commentId, MakoClient makoClient)
    : AbstractPixivFetchEngine<Comment>(makoClient)
{
    public override IAsyncEnumerator<Comment> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Comment<NovelCommentRepliesEngine>(
            this,
            $"/v2/novel/comment/replies" +
            $"?comment_id={commentId}");
}
