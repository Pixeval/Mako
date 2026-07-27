// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

[method: MakoExtensionConstructor(true)]
internal class BookmarkUserIllustrationEngine(long illustrationId, MakoClient makoClient)
    : AbstractPixivFetchEngine<BookmarkUserInfo>(makoClient)
{
    public override IAsyncEnumerator<BookmarkUserInfo> GetAsyncEnumerator(CancellationToken cancellationToken = new()) =>
        new RecursivePixivAsyncEnumerators.BookmarkUserInfo<BookmarkUserIllustrationEngine>(
            this,
            "/v1/illust/bookmark/users"
            + $"?{TargetFilterParam}"
            + $"&illust_id={illustrationId}");
}
