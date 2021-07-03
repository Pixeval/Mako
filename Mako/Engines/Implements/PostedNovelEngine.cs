using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines.Implements
{
    public class PostedNovelEngine : AbstractPixivFetchEngine<Novel>
    {
        private readonly TargetFilter _targetFilter;
        private readonly string _uid;

        public PostedNovelEngine(
            [NotNull] MakoClient makoClient,
            string uid,
            TargetFilter targetFilter,
            EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _uid = uid;
            _targetFilter = targetFilter;
        }

        public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return RecursivePixivAsyncEnumerators.Novel<PostedNovelEngine>.WithInitialUrl(this, MakoApiKind.AppApi,
                engine => $"/v1/user/novels?user_id={engine._uid}&filter={engine._targetFilter.GetDescription()}")!;
        }
    }
}