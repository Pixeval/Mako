// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <summary>
/// Request followers of a user.
/// </summary>
/// <returns>
/// The <see cref="UserFollowerEngine" /> containing follower users.
/// </returns>
[method: MakoExtensionConstructor]
internal class UserFollowerEngine(MakoClient makoClient) : AbstractPixivFetchEngine<User>(makoClient)
{
    public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = new()) =>
        new RecursivePixivAsyncEnumerators.User<UserFollowerEngine>(
            this,
            "/v1/user/follower");
}
