using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;

namespace Mako.Engines.Implements
{
    internal class UserUploadEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly string _uid;
        
        public UserUploadEngine([NotNull] MakoClient makoClient, string uid, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _uid = uid;
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new UserUploadAsyncEnumerable(this, MakoApiKind.AppApi)!;
        }

        private class UserUploadAsyncEnumerable : RecursivePixivAsyncEnumerators.Illustration<UserUploadEngine>
        {
            public UserUploadAsyncEnumerable([NotNull] UserUploadEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override string InitialUrl() => $"/v1/user/illusts?user_id={PixivFetchEngine._uid}&filter=for_android&type=illust";
        }
    }
}