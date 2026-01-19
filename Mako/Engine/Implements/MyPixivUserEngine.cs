// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;

namespace Mako.Engine.Implements;

internal class MyPixivUserEngine(MakoClient makoClient, long userId, EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<User>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.User<MyPixivUserEngine>(
            this,
            "/v1/user/mypixiv" +
            $"?user_id={userId}");
}
