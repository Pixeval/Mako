// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <summary>
/// Request replies of a comment.
/// </summary>
/// <param name="commentId">Comment id</param>
/// <returns>
/// The <see cref="IllustrationCommentRepliesEngine" /> containing replies of the comment.
/// </returns>
[method: MakoExtensionConstructor]
internal class IllustrationCommentRepliesEngine(long commentId, MakoClient makoClient)
    : AbstractPixivFetchEngine<Comment>(makoClient)
{
    public override IAsyncEnumerator<Comment> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Comment<IllustrationCommentRepliesEngine>(
            this,
            "/v2/illust/comment/replies" +
            $"?comment_id={commentId}");
}
