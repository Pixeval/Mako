using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines.Implements
{
    public class NovelBookmarkEngine : AbstractPixivFetchEngine<Novel>
    {
        private readonly PrivacyPolicy _privacyPolicy;
        private readonly TargetFilter _targetFilter;
        private readonly string _uid;

        public NovelBookmarkEngine(
            [NotNull] MakoClient makoClient,
            string uid,
            PrivacyPolicy privacyPolicy,
            TargetFilter targetFilter,
            EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _uid = uid;
            _privacyPolicy = privacyPolicy;
            _targetFilter = targetFilter;
        }

        public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return RecursivePixivAsyncEnumerators.Novel<NovelBookmarkEngine>.WithInitialUrl(this, MakoApiKind.AppApi,
                engine => $"/v1/user/bookmarks/novel?user_id={engine._uid}&restrict={engine._privacyPolicy.GetDescription()}&filter={engine._targetFilter.GetDescription()}")!;
        }
    }
}