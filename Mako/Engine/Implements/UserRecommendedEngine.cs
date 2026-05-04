// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <summary>
/// Request recommended users.
/// </summary>
/// <returns>
/// The <see cref="UserRecommendedEngine" /> containing recommended users.
/// </returns>
[method: MakoExtensionConstructor]
internal class UserRecommendedEngine(MakoClient makoClient)
    : AbstractPixivFetchEngine<User>(makoClient)
{
    public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.User<UserRecommendedEngine>(
            this,
            "/v1/user/recommended" +
            $"?{TargetFilterParam}");
}
