// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

public class UserSearchEngine(MakoClient makoClient, TargetFilter targetFilter,
        string keyword, EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<User>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.User<UserSearchEngine>(
            this,
            "/v1/search/user" +
            $"?filter={targetFilter.GetDescription()}" +
            $"&word={keyword}");
}
