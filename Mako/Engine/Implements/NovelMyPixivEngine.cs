// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <inheritdoc cref="IllustrationMyPixivEngine.IllustrationMyPixivEngine" />
[method: MakoExtensionConstructor(true)]
internal class NovelMyPixivEngine(MakoClient makoClient)
    : AbstractPixivFetchEngine<Novel>(makoClient)
{
    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<NovelMyPixivEngine>(
            this,
            "/v2/novel/mypixiv");
}
