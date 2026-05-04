// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

[method: MakoExtensionConstructor]
internal class IllustrationNewEngine(
    MakoClient makoClient,
    bool contentTypeIsManga,
    uint? maxIllustrationId)
    : AbstractPixivFetchEngine<Illustration>(makoClient)
{
    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<IllustrationNewEngine>(this,
            "/v1/illust/new"
            + $"?content_type={(contentTypeIsManga ? "manga" : "illust")}"
            + $"&{TargetFilterParam}"
            + maxIllustrationId?.Let(static s => $"&max_illust_id={s}"));
}
