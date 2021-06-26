using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
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
            return new UserUpdateAsyncEnumerator(this, MakoApiKind.AppApi)!;
        }

        private class UserUpdateAsyncEnumerator : RecursivePixivAsyncEnumerator<Illustration, PixivResponse, UserUpdateEngine>
        {
            public UserUpdateAsyncEnumerator([NotNull] UserUpdateEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override bool ValidateResponse(PixivResponse rawEntity)
            {
                return rawEntity.Illusts.IsNotNullOrEmpty();
            }

            protected override string? NextUrl(PixivResponse? rawEntity)
            {
                return rawEntity?.NextUrl;
            }

            protected override string InitialUrl()
            {
                return $"/v2/illust/follow?restrict={PixivFetchEngine._privacyPolicy.GetDescription()}";
            }

            protected override IEnumerator<Illustration>? GetNewEnumerator(PixivResponse? rawEntity)
            {
                return rawEntity?.Illusts.SelectNotNull(MakoExtension.ToIllustration).GetEnumerator();
            }
        }
    }
}