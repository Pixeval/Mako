using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Mako.Util;

namespace Mako.Engines.Implements
{
    internal class UserFollowingEngine : AbstractPixivFetchEngine<User>
    {
        private readonly PrivacyPolicy _privacyPolicy;
        private readonly string _uid;
        
        public UserFollowingEngine([NotNull] MakoClient makoClient, PrivacyPolicy privacyPolicy, string uid, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _privacyPolicy = privacyPolicy;
            _uid = uid;
        }

        public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new UserFollowingAsyncEnumerator(this, MakoApiKind.AppApi)!;
        }

        private class UserFollowingAsyncEnumerator : RecursivePixivAsyncEnumerator<User, PixivUserResponse, UserFollowingEngine>
        {
            public UserFollowingAsyncEnumerator([NotNull] UserFollowingEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
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
                return $"/v1/user/following?user_id={PixivFetchEngine._uid}&restrict={PixivFetchEngine._privacyPolicy.GetDescription()}";
            }

            protected override IEnumerator<User>? GetNewEnumerator(PixivUserResponse? rawEntity)
            {
                return rawEntity?.Users?.SelectNotNull(MakoExtension.ToUserIncomplete).GetEnumerator();
            }
        }
    }
}