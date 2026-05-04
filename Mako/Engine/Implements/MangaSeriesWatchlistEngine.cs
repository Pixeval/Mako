// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

[method: MakoExtensionConstructor]
internal class MangaSeriesWatchlistEngine(MakoClient makoClient)
    : AbstractPixivFetchEngine<Series>(makoClient)
{
    public override IAsyncEnumerator<Series> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Series<MangaSeriesWatchlistEngine>(
            this,
            "/v1/watchlist/manga");
}
