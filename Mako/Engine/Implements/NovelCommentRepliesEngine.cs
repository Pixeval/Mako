// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;

namespace Mako.Engine.Implements;

public class NovelCommentRepliesEngine(string commentId, MakoClient makoClient, EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<Comment>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<Comment> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Comment<NovelCommentRepliesEngine>(
            this,
            $"/v2/novel/comment/replies" +
            $"?comment_id={commentId}");
}
