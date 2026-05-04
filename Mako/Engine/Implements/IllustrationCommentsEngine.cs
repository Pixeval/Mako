// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <summary>
/// Request comments of an illustration.
/// </summary>
/// <returns>
/// The <see cref="IllustrationCommentsEngine" /> containing comments of the illustration.
/// </returns>
[method: MakoExtensionConstructor]
internal class IllustrationCommentsEngine(long illustrationId, MakoClient makoClient)
    : AbstractPixivFetchEngine<Comment>(makoClient)
{
    public override IAsyncEnumerator<Comment> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Comment<IllustrationCommentsEngine>(
            this,
            $"/v3/illust/comments" +
            $"?illust_id={illustrationId}");
}
