using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Net.Responses;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <inheritdoc cref="MangaSeriesEngine.MangaSeriesEngine" />
[method: MakoExtensionConstructor(true)]
internal class NovelSeriesEngine(MakoClient makoClient, long novelSeriesId, NovelSeriesDetailResponse? current = null)
    : AbstractPixivFetchEngine<Novel>(makoClient)
{
    public const string UrlSegment = "/v2/novel/series";

    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var engine = new RecursivePixivAsyncEnumerators.Novel<NovelSeriesEngine>(
            this,
            UrlSegment
            + $"?series_id={novelSeriesId}");
        if (current is not null)
            engine.Update(current);
        return engine;
    }
}
