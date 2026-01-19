// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;

namespace Mako.Engine.Implements;

public class IllustrationCommentsEngine(long illustId, MakoClient makoClient, EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<Comment>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<Comment> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Comment<IllustrationCommentsEngine>(
            this,
            $"/v3/illust/comments" +
            $"?illust_id={illustId}");
}
