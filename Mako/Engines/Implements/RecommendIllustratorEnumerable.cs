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
        private readonly TargetFilter _targetFilter;

        public RecommendIllustratorEngine(MakoClient makoClient, TargetFilter targetFilter, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _targetFilter = targetFilter;
        }

        public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new RecommendIllustratorAsyncEnumerator(this, MakoApiKind.AppApi)!;
        }

        private class RecommendIllustratorAsyncEnumerator : RecursivePixivAsyncEnumerators.User<RecommendIllustratorEngine>
        {
            public RecommendIllustratorAsyncEnumerator([NotNull] RecommendIllustratorEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override string InitialUrl()
            {
                return $"/v1/user/recommended?filter={PixivFetchEngine._targetFilter.GetDescription()}";
            }
        }
    }
}