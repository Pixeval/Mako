using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Net.Responses;
using Mako.Utilities;

namespace Mako.Engine.Implements;

[method: MakoExtensionConstructor(true)]
internal class MangaSeriesEngine(MakoClient makoClient, long mangaSeriesId, MangaSeriesDetailResponse? current = null)
    : AbstractPixivFetchEngine<Illustration>(makoClient)
{
    public const string UrlSegment = "/v1/illust/series";

    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var engine = new RecursivePixivAsyncEnumerators.Illustration<MangaSeriesEngine>(
            this,
            UrlSegment
            + $"?{TargetFilterParam}"
            + $"&illust_series_id={mangaSeriesId}");
        if (current is not null)
            engine.Update(current);
        return engine;
    }
}
