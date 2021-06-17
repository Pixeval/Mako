﻿using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Mako.Util;

namespace Mako.Engines.Implements
{
    internal class RankingEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly RankOption _rankOption;
        private readonly DateTime _dateTime;
        
        public sealed override MakoClient MakoClient { get; set; } 
        
        
        public RankingEngine(MakoClient makoClient, RankOption rankOption, DateTime dateTime, EngineHandle? engineHandle) : base(engineHandle)
        {
            MakoClient = makoClient;
            _rankOption = rankOption;
            _dateTime = dateTime;
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new RankingAsyncEnumerator(this, MakoApiKind.AppApi, MakoClient)!;
        }

        private class RankingAsyncEnumerator : RecursivePixivAsyncEnumerator<Illustration, PixivResponse, RankingEngine>
        {
            public RankingAsyncEnumerator([NotNull] RankingEngine pixivFetchEngine, MakoApiKind makoApiKind, [NotNull] MakoClient makoClient) : base(pixivFetchEngine, makoApiKind, makoClient)
            {
            }

            protected override bool ValidateResponse(PixivResponse rawEntity)
            {
                return rawEntity.Illusts.IsNotNullOrEmpty();
            }

            protected override string? NextUrl() => Entity?.NextUrl;

            protected override string InitialUrl() => $"/v1/illust/ranking?filter=for_android&mode={PixivFetchEngine._rankOption.GetDescription()}&date={PixivFetchEngine._dateTime:yyyy-MM-dd}";
            
            protected override IEnumerator<Illustration> GetNewEnumerator()
            {
                return (Entity?.Illusts!.SelectNotNull(MakoExtension.ToIllustration) ?? Array.Empty<Illustration>()).GetEnumerator();
            }
        }
    }
}