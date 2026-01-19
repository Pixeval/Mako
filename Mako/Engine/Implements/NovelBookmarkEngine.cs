// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Web;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

public class NovelBookmarkEngine(
    MakoClient makoClient,
    long uid,
    string? tag,
    PrivacyPolicy privacyPolicy,
    TargetFilter targetFilter,
    EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<Novel>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<NovelBookmarkEngine>(
            this,
            "/v1/user/bookmarks/novel"
            + $"?user_id={uid}"
            + $"&restrict={privacyPolicy.GetDescription()}"
            + $"&filter={targetFilter.GetDescription()}"
            + tag?.Let(s => $"&tag={HttpUtility.UrlEncode(s)}"));
}
