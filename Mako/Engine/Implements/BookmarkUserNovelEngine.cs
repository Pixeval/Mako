// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

[method: MakoExtensionConstructor(true)]
internal class BookmarkUserNovelEngine(long novelId, MakoClient makoClient)
    : AbstractPixivFetchEngine<BookmarkUserInfo>(makoClient)
{
    public override IAsyncEnumerator<BookmarkUserInfo> GetAsyncEnumerator(CancellationToken cancellationToken = new()) =>
        new RecursivePixivAsyncEnumerators.BookmarkUserInfo<BookmarkUserNovelEngine>(
            this,
            "/v1/novel/bookmark/users"
            + $"?{TargetFilterParam}"
            + $"&novel_id={novelId}");
}
