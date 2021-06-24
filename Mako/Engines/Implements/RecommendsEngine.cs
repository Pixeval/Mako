﻿using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Mako.Util;

namespace Mako.Engines.Implements
{
    internal class RecommendsEngine : AbstractPixivFetchEngine<Illustration>
    {
        
        public RecommendsEngine(MakoClient makoClient, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new RecommendsAsyncEnumerator(this, MakoApiKind.AppApi)!;
        }

        private class RecommendsAsyncEnumerator : RecursivePixivAsyncEnumerator<Illustration, PixivResponse, RecommendsEngine>
        {
            public RecommendsAsyncEnumerator([NotNull] RecommendsEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override bool ValidateResponse(PixivResponse rawEntity)
            {
                return rawEntity.Illusts.IsNotNullOrEmpty();
            }

            protected override string? NextUrl(PixivResponse? rawEntity) => rawEntity?.NextUrl;

            protected override string InitialUrl() => "/v1/illust/recommended";

            protected override IEnumerator<Illustration>? GetNewEnumerator(PixivResponse? rawEntity)
            {
                return rawEntity?.Illusts?.SelectNotNull(MakoExtension.ToIllustration).GetEnumerator();
            }
        }
    }
}