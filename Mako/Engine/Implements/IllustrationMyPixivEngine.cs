// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

[method: MakoExtensionConstructor]
internal class IllustrationMyPixivEngine(MakoClient makoClient, long userId)
    : AbstractPixivFetchEngine<Illustration>(makoClient)
{
    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<IllustrationMyPixivEngine>(
            this,
            "/v2/illust/mypixiv" +
            $"?user_id={userId}");
}
