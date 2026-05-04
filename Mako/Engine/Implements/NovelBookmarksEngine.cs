// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Web;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class NovelBookmarksEngine : AbstractPixivFetchEngine<Novel>
{
    private readonly long _uid;
    private readonly string? _tag;
    private readonly PrivacyPolicy _privacyPolicy;

    /// <inheritdoc cref="IllustrationBookmarksEngine.IllustrationBookmarksEngine" />
    [MakoExtensionConstructor]
    public NovelBookmarksEngine(MakoClient makoClient,
        long uid,
        string? tag,
        PrivacyPolicy privacyPolicy) : base(makoClient)
    {
        makoClient.CheckPrivacyPolicy(privacyPolicy, uid);

        _uid = uid;
        _tag = tag;
        _privacyPolicy = privacyPolicy;
    }

    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Novel<NovelBookmarksEngine>(
            this,
            "/v1/user/bookmarks/novel"
            + $"?user_id={_uid}"
            + $"&restrict={_privacyPolicy.GetDescription()}"
            + $"&{TargetFilterParam}"
            + _tag?.Let(s => $"&tag={HttpUtility.UrlEncode(s)}"));
}
