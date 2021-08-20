﻿#region Copyright (c) Pixeval/Mako

// MIT License
// 
// Copyright (c) Pixeval 2021 Mako/RecursivePixivFetchEngine.cs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Mako.Util;

namespace Mako.Engine
{
    internal abstract class RecursivePixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine> : AbstractPixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine>
        where TEntity : class
        where TFetchEngine : class, IFetchEngine<TEntity>
    {
        protected RecursivePixivAsyncEnumerator(TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind)
            : base(pixivFetchEngine, makoApiKind)
        {
        }

        private TRawEntity? RawEntity { get; set; }

        protected abstract string? NextUrl(TRawEntity? rawEntity);

        protected abstract string InitialUrl();

        protected abstract IEnumerator<TEntity>? GetNewEnumerator(TRawEntity? rawEntity);

        protected virtual bool HasNextPage()
        {
            return NextUrl(RawEntity).IsNotNullOrEmpty();
        }

        public override async ValueTask<bool> MoveNextAsync()
        {
            if (IsCancellationRequested)
            {
                PixivFetchEngine.EngineHandle.Complete(); // Set the state of the 'PixivFetchEngine' to Completed
                return false;
            }

            if (RawEntity is null)
            {
                var first = InitialUrl();
                switch (await GetJsonResponseAsync(first).ConfigureAwait(false))
                {
                    case Result<TRawEntity>.Success (var raw):
                        Update(raw);
                        break;
                    case Result<TRawEntity>.Failure (var exception):
                        if (exception is { } e)
                        {
                            throw e;
                        }

                        PixivFetchEngine.EngineHandle.Complete();
                        return false;
                }
            }

            if (CurrentEntityEnumerator!.MoveNext()) // If the enumerator can proceeds then return true
            {
                return true;
            }

            if (!HasNextPage()) // Check if there are more pages, return false if not
            {
                PixivFetchEngine.EngineHandle.Complete();
                return false;
            }

            if (await GetJsonResponseAsync(NextUrl(RawEntity)!).ConfigureAwait(false) is Result<TRawEntity>.Success (var value)) // Else request a new page
            {
                if (IsCancellationRequested)
                {
                    PixivFetchEngine.EngineHandle.Complete();
                    return false;
                }
                Update(value);
                return true;
            }

            PixivFetchEngine.EngineHandle.Complete();
            return false;
        }

        private void Update(TRawEntity rawEntity)
        {
            RawEntity = rawEntity;
            CurrentEntityEnumerator = GetNewEnumerator(rawEntity) ?? EmptyEnumerators<TEntity>.Sync;
            PixivFetchEngine!.RequestedPages++;
        }
    }

    internal static class RecursivePixivAsyncEnumerators
    {
        public abstract class User<TFetchEngine> : RecursivePixivAsyncEnumerator<User, PixivUserResponse, TFetchEngine>
            where TFetchEngine : class, IFetchEngine<User>
        {
            protected User([NotNull] TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
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

            protected abstract override string InitialUrl();

            protected override IEnumerator<User>? GetNewEnumerator(PixivUserResponse? rawEntity)
            {
                var tasks = rawEntity?.Users;
                return tasks?.GetEnumerator();
            }

            public static User<TFetchEngine> WithInitialUrl(TFetchEngine engine, MakoApiKind kind, Func<TFetchEngine, string> initialUrlFactory)
            {
                return new UserImpl<TFetchEngine>(engine, kind, initialUrlFactory);
            }
        }

        private class UserImpl<TFetchEngine> : User<TFetchEngine>
            where TFetchEngine : class, IFetchEngine<User>
        {
            private readonly Func<TFetchEngine, string> _initialUrlFactory;

            public UserImpl([NotNull] TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind, Func<TFetchEngine, string> initialUrlFactory) : base(pixivFetchEngine, makoApiKind)
            {
                _initialUrlFactory = initialUrlFactory;
            }

            protected override string InitialUrl()
            {
                return _initialUrlFactory(PixivFetchEngine);
            }
        }

        public abstract class Illustration<TFetchEngine> : RecursivePixivAsyncEnumerator<Illustration, PixivResponse, TFetchEngine>
            where TFetchEngine : class, IFetchEngine<Illustration>
        {
            protected Illustration([NotNull] TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
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

            protected abstract override string InitialUrl();

            protected override IEnumerator<Illustration>? GetNewEnumerator(PixivResponse? rawEntity)
            {
                return rawEntity?.Illusts?.GetEnumerator();
            }

            public static Illustration<TFetchEngine> WithInitialUrl(TFetchEngine engine, MakoApiKind kind, Func<TFetchEngine, string> initialUrlFactory)
            {
                return new IllustrationImpl<TFetchEngine>(engine, kind, initialUrlFactory);
            }
        }

        private class IllustrationImpl<TFetchEngine> : Illustration<TFetchEngine>
            where TFetchEngine : class, IFetchEngine<Illustration>
        {
            private readonly Func<TFetchEngine, string> _initialUrlFactory;

            public IllustrationImpl([NotNull] TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind, Func<TFetchEngine, string> initialUrlFactory) : base(pixivFetchEngine, makoApiKind)
            {
                _initialUrlFactory = initialUrlFactory;
            }

            protected override string InitialUrl()
            {
                return _initialUrlFactory(PixivFetchEngine);
            }
        }

        public abstract class Novel<TFetchEngine> : RecursivePixivAsyncEnumerator<Novel, PixivNovelResponse, TFetchEngine>
            where TFetchEngine : class, IFetchEngine<Novel>
        {
            protected Novel([NotNull] TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override bool ValidateResponse(PixivNovelResponse rawEntity)
            {
                return rawEntity.Novels.IsNotNullOrEmpty();
            }

            protected override string? NextUrl(PixivNovelResponse? rawEntity)
            {
                return rawEntity?.NextUrl;
            }

            protected abstract override string InitialUrl();

            protected override IEnumerator<Novel>? GetNewEnumerator(PixivNovelResponse? rawEntity)
            {
                return rawEntity?.Novels?.GetEnumerator();
            }

            public static Novel<TFetchEngine> WithInitialUrl(TFetchEngine engine, MakoApiKind kind, Func<TFetchEngine, string> initialUrlFactory)
            {
                return new NovelImpl<TFetchEngine>(engine, kind, initialUrlFactory);
            }
        }

        private class NovelImpl<TFetchEngine> : Novel<TFetchEngine>
            where TFetchEngine : class, IFetchEngine<Novel>
        {
            private readonly Func<TFetchEngine, string> _initialUrlFactory;

            public NovelImpl([NotNull] TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind, Func<TFetchEngine, string> initialUrlFactory) : base(pixivFetchEngine, makoApiKind)
            {
                _initialUrlFactory = initialUrlFactory;
            }

            protected override string InitialUrl()
            {
                return _initialUrlFactory(PixivFetchEngine);
            }
        }

        public abstract class Comment<TFetchEngine> : RecursivePixivAsyncEnumerator<IllustrationCommentsResponse.Comment, IllustrationCommentsResponse, TFetchEngine>
            where TFetchEngine : class, IFetchEngine<IllustrationCommentsResponse.Comment>
        {
            protected Comment([NotNull] TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override bool ValidateResponse(IllustrationCommentsResponse rawEntity)
            {
                return rawEntity.Comments.IsNotNullOrEmpty();
            }

            protected override string? NextUrl(IllustrationCommentsResponse? rawEntity)
            {
                return rawEntity?.NextUrl;
            }

            protected abstract override string InitialUrl();

            protected override IEnumerator<IllustrationCommentsResponse.Comment>? GetNewEnumerator(IllustrationCommentsResponse? rawEntity)
            {
                return rawEntity?.Comments?.GetEnumerator();
            }

            public static Comment<TFetchEngine> WithInitialUrl(TFetchEngine engine, MakoApiKind kind, Func<TFetchEngine, string> initialUrlFactory)
            {
                return new CommentImpl<TFetchEngine>(engine, kind, initialUrlFactory);
            }
        }

        private class CommentImpl<TFetchEngine> : Comment<TFetchEngine>
            where TFetchEngine : class, IFetchEngine<IllustrationCommentsResponse.Comment>
        {
            private readonly Func<TFetchEngine, string> _initialUrlFactory;

            public CommentImpl([NotNull] TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind, Func<TFetchEngine, string> initialUrlFactory) : base(pixivFetchEngine, makoApiKind)
            {
                _initialUrlFactory = initialUrlFactory;
            }

            protected override string InitialUrl()
            {
                return _initialUrlFactory(PixivFetchEngine);
            }
        }
    }
}