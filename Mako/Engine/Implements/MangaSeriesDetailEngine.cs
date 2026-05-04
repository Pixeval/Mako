using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Net.Response;

namespace Mako.Engine.Implements;

internal class MangaSeriesDetailEngine(MakoClient makoClient, long mangaSeriesId, MangaSeriesDetailResponse current)
    : AbstractPixivFetchEngine<Illustration>(makoClient)
{
    public const string UrlSegment = "/v1/illust/series";

    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var engine = new RecursivePixivAsyncEnumerators.Illustration<MangaSeriesDetailEngine>(
            this,
            UrlSegment
            + $"?{TargetFilterParam}"
            + $"&illust_series_id={mangaSeriesId}");
        engine.Update(current);
        return engine;
    }
}
