using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Net.Response;

namespace Mako.Engine.Implements;

internal class NovelSeriesDetailEngine(MakoClient makoClient, long novelSeriesId, NovelSeriesDetailResponse current)
    : AbstractPixivFetchEngine<Novel>(makoClient)
{
    public const string UrlSegment = "/v2/novel/series";

    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var engine = new RecursivePixivAsyncEnumerators.Novel<NovelSeriesDetailEngine>(
            this,
            UrlSegment
            + $"?series_id={novelSeriesId}");
        engine.Update(current);
        return engine;
    }
}
