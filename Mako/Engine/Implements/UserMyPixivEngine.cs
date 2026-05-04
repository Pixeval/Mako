// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

[method: MakoExtensionConstructor]
internal class UserMyPixivEngine(MakoClient makoClient, long userId)
    : AbstractPixivFetchEngine<User>(makoClient)
{
    public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.User<UserMyPixivEngine>(
            this,
            "/v1/user/mypixiv" +
            $"?user_id={userId}");
}
