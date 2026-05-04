// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <summary>
/// Request the spotlights in Pixiv.
/// </summary>
/// <returns>
/// The <see cref="SpotlightEngine" /> containing the spotlight articles.
/// </returns>
[method: MakoExtensionConstructor]
internal class SpotlightEngine(MakoClient makoClient,
    SpotlightCategory category = SpotlightCategory.All)
    : AbstractPixivFetchEngine<Spotlight>(makoClient)
{
    public override IAsyncEnumerator<Spotlight> GetAsyncEnumerator(
        CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Spotlight<SpotlightEngine>(
            this,
            "/v1/spotlight/articles"
            + $"?{TargetFilterParam}"
            + $"&category={category.GetDescription()}");
}
