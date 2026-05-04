// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <inheritdoc cref="IllustrationFollowingEngine.IllustrationFollowingEngine" />
[method: MakoExtensionConstructor]
internal class NovelFollowingEngine(MakoClient makoClient, PrivacyPolicy privacyPolicy)
    : AbstractPixivFetchEngine<Novel>(makoClient)
{
    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<NovelFollowingEngine>(
            this,
            "/v1/novel/follow"
            + $"?restrict={privacyPolicy.GetDescription()}");
}
