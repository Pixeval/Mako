using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Mako.Util;

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

        private class UserUploadAsyncEnumerable : RecursivePixivAsyncEnumerator<Illustration, PixivResponse, UserUploadEngine>
        {
            public UserUploadAsyncEnumerable([NotNull] UserUploadEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override bool ValidateResponse(PixivResponse rawEntity)
            {
                return rawEntity.Illusts.IsNotNullOrEmpty();
            }

            protected override string? NextUrl(PixivResponse? rawEntity) => rawEntity?.NextUrl;

            protected override string InitialUrl() => $"/v1/user/illusts?user_id={PixivFetchEngine._uid}&filter=for_android&type=illust";

            protected override IEnumerator<Illustration>? GetNewEnumerator(PixivResponse? rawEntity)
            {
                return rawEntity?.Illusts?.SelectNotNull(MakoExtension.ToIllustration).GetEnumerator();
            }
        }
    }
}