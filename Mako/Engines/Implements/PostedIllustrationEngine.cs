using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;

namespace Mako.Engines.Implements
{
    internal class PostedIllustrationEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly string _uid;

        public PostedIllustrationEngine([NotNull] MakoClient makoClient, string uid, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _uid = uid;
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return RecursivePixivAsyncEnumerators.Illustration<PostedIllustrationEngine>.WithInitialUrl(this, MakoApiKind.AppApi, engine => $"/v1/user/illusts?user_id={engine._uid}&filter=for_android&type=illust")!;
        }
    }
}