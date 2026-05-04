// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;


internal class IllustrationBookmarkTagsEngine : AbstractPixivFetchEngine<BookmarkTag>
{
    private readonly long _uid;
    private readonly PrivacyPolicy _privacyPolicy;

    [MakoExtensionConstructor]
    public IllustrationBookmarkTagsEngine(MakoClient makoClient, long uid, PrivacyPolicy privacyPolicy) : base(makoClient)
    {
        makoClient.CheckPrivacyPolicy(privacyPolicy, uid);

        _uid = uid;
        _privacyPolicy = privacyPolicy;
    }

    public override IAsyncEnumerator<BookmarkTag> GetAsyncEnumerator(CancellationToken cancellationToken = new()) =>
        new RecursivePixivAsyncEnumerators.BookmarkTag<IllustrationBookmarkTagsEngine>(
            this,
            "/v1/user/bookmark-tags/illust"
            + $"?user_id={_uid}"
            + $"&restrict={_privacyPolicy.GetDescription()}");
}
