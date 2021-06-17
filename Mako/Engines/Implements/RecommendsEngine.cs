using System;
using System.Collections.Generic;
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
        public sealed override MakoClient MakoClient { get; set; }
        
        public RecommendsEngine(MakoClient makoClient, EngineHandle? engineHandle) : base(engineHandle)
        {
            MakoClient = makoClient;
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new RecommendsAsyncEnumerator(this, MakoApiKind.AppApi, MakoClient)!;
        }

        private class RecommendsAsyncEnumerator : RecursivePixivAsyncEnumerator<Illustration, PixivResponse, RecommendsEngine>
        {
            public RecommendsAsyncEnumerator([NotNull] RecommendsEngine pixivFetchEngine, MakoApiKind makoApiKind, [NotNull] MakoClient makoClient) : base(pixivFetchEngine, makoApiKind, makoClient)
            {
            }

            protected override bool ValidateResponse(PixivResponse rawEntity)
            {
                return rawEntity.Illusts.IsNotNullOrEmpty();
            }

            protected override string? NextUrl() => Entity?.NextUrl;

            protected override string InitialUrl() => "/v1/illust/recommended";

            protected override IEnumerator<Illustration> GetNewEnumerator()
            {
                return (Entity?.Illusts!.SelectNotNull(MakoExtension.ToIllustration) ?? Array.Empty<Illustration>()).GetEnumerator();
            }
        }
    }
}