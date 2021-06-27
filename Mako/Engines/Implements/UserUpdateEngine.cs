using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines.Implements
{
    public class UserUpdateEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly PrivacyPolicy _privacyPolicy;
        
        public UserUpdateEngine([NotNull] MakoClient makoClient, PrivacyPolicy privacyPolicy, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _privacyPolicy = privacyPolicy;
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return RecursivePixivAsyncEnumerators.Illustration<UserUpdateEngine>.WithInitialUrl(this, MakoApiKind.AppApi, engine => $"/v2/illust/follow?restrict={engine._privacyPolicy.GetDescription()}")!;
        }
    }
}