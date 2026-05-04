// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <summary>
/// Search user in Pixiv.
/// </summary>
/// <param name="keyword">The text in searching</param>
/// <returns>
/// The <see cref="IFetchEngine{T}" /> containing the search results for users.
/// </returns>
[method: MakoExtensionConstructor]
internal class UserSearchEngine(
    MakoClient makoClient,
    string keyword) : AbstractPixivFetchEngine<User>(makoClient)
{
    public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.User<UserSearchEngine>(
            this,
            "/v1/search/user" +
            $"?{TargetFilterParam}" +
            $"&word={keyword}");
}
