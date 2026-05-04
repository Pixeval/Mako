// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

[method: MakoExtensionConstructor]
internal class IllustrationRelatedEngine(long illustrationId, MakoClient makoClient)
    : AbstractPixivFetchEngine<Illustration>(makoClient)
{
    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<IllustrationRelatedEngine>(
            this,
            "/v2/illust/related"
            + $"?{TargetFilterParam}"
            + $"&illust_id={illustrationId}");
}
