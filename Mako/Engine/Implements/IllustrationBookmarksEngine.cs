// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Web;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class IllustrationBookmarksEngine : AbstractPixivFetchEngine<Illustration>
{
    private readonly long _uid;
    private readonly string? _tag;
    private readonly PrivacyPolicy _privacyPolicy;

    /// <summary>
    /// An <see cref="IFetchEngine{E}" /> that fetches the bookmark of a specific user
    /// </summary>
    /// <remarks>
    /// Creates a <see cref="IllustrationBookmarksEngine" />
    /// </remarks>
    /// <param name="makoClient">The <see cref="MakoClient" /> that owns this object</param>
    /// <param name="uid">ID of the user</param>
    /// <param name="tag"></param>
    /// <param name="privacyPolicy">The privacy option</param>
    [MakoExtensionConstructor]
    public IllustrationBookmarksEngine(MakoClient makoClient,
        long uid,
        string? tag,
        PrivacyPolicy privacyPolicy) : base(makoClient)
    {
        makoClient.CheckPrivacyPolicy(privacyPolicy, uid);

        _uid = uid;
        _tag = tag;
        _privacyPolicy = privacyPolicy;
    }

    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<IllustrationBookmarksEngine>(
            this,
            "/v1/user/bookmarks/illust"
            + $"?user_id={_uid}"
            + $"&restrict={_privacyPolicy.GetDescription()}"
            + $"&{TargetFilterParam}"
            + _tag?.Let(s => $"&tag={HttpUtility.UrlEncode(s)}"));
}
