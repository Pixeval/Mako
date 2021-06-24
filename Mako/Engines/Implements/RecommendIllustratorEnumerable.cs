using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Mako.Util;

namespace Mako.Engines.Implements
{
    internal class RecommendIllustratorEngine : AbstractPixivFetchEngine<User>
    {
        public RecommendIllustratorEngine(MakoClient makoClient, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
        }

        public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new RecommendIllustratorAsyncEnumerator(this, MakoApiKind.AppApi)!;
        }

        private class RecommendIllustratorAsyncEnumerator : RecursivePixivAsyncEnumerator<User, PixivUserResponse, RecommendIllustratorEngine>
        {
            public RecommendIllustratorAsyncEnumerator([NotNull] RecommendIllustratorEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override bool ValidateResponse(PixivUserResponse rawEntity)
            {
                return rawEntity.Users.IsNotNullOrEmpty();
            }

            protected override string? NextUrl(PixivUserResponse? rawEntity)
            {
                return rawEntity?.NextUrl;
            }

            protected override string InitialUrl()
            {
                return "/v1/user/recommended?filter=for_android";
            }

            protected override IEnumerator<User>? GetNewEnumerator(PixivUserResponse? rawEntity)
            {
                return rawEntity?.Users?.SelectNotNull(MakoExtension.ToUserIncomplete).GetEnumerator();
            }
        }
    }
}