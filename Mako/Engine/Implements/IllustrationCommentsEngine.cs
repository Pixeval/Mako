using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Net;
using Mako.Net.Response;

namespace Mako.Engine.Implements
{
    public class IllustrationCommentsEngine : AbstractPixivFetchEngine<IllustrationCommentsResponse.Comment>
    {
        private readonly string _illustId;

        public IllustrationCommentsEngine(string illustId, [NotNull] MakoClient makoClient, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _illustId = illustId;
        }

        public override IAsyncEnumerator<IllustrationCommentsResponse.Comment> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return RecursivePixivAsyncEnumerators.Comment<IllustrationCommentsEngine>.WithInitialUrl(this, MakoApiKind.AppApi,
                engine => $"/v3/illust/comments?illust_id={engine._illustId}")!;
        }
    }
}