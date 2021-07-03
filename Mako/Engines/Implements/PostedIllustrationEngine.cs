﻿#region Copyright (c) Pixeval/Mako

// MIT License
// 
// Copyright (c) Pixeval 2021 Mako/PostedIllustrationEngine.cs
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

using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;

namespace Mako.Engines.Implements
{
    internal class PostedIllustrationEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly string _uid;

        public PostedIllustrationEngine([NotNull] MakoClient makoClient, string uid, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _uid = uid;
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return RecursivePixivAsyncEnumerators.Illustration<PostedIllustrationEngine>.WithInitialUrl(this, MakoApiKind.AppApi, engine => $"/v1/user/illusts?user_id={engine._uid}&filter=for_android&type=illust")!;
        }
    }
}