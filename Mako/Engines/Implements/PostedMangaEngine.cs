using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines.Implements
{
    public class PostedMangaEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly TargetFilter _targetFilter;
        private readonly string _uid;

        public PostedMangaEngine([NotNull] MakoClient makoClient, string uid, TargetFilter targetFilter, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _uid = uid;
            _targetFilter = targetFilter;
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return RecursivePixivAsyncEnumerators.Illustration<PostedMangaEngine>.WithInitialUrl(this, MakoApiKind.AppApi,
                engine => $"/v1/user/illusts?filter={_targetFilter.GetDescription()}&user_id={engine._uid}&type=manga")!;
        }
    }
}