using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Net;
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
            return RecursivePixivAsyncEnumerators.User<RecommendIllustratorEngine>.WithInitialUrl(this, MakoApiKind.AppApi, engine => $"/v1/user/recommended?filter={engine._targetFilter.GetDescription()}")!;
        }
    }
}