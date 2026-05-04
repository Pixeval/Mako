// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <inheritdoc cref="IllustrationPostedEngine.IllustrationPostedEngine" />
[method: MakoExtensionConstructor]
internal class NovelPostedEngine(MakoClient makoClient, long uid)
    : AbstractPixivFetchEngine<Novel>(makoClient)
{
    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<NovelPostedEngine>(this,
            "/v1/user/novels" +
            $"?user_id={uid}" +
            $"&{TargetFilterParam}");
}
