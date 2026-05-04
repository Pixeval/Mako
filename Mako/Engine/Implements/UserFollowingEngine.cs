// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class UserFollowingEngine : AbstractPixivFetchEngine<User>
{
    private readonly long _uid;
    private readonly PrivacyPolicy _privacyPolicy;

    /// <summary>
    /// Request following users of a user.
    /// </summary>
    /// <returns>
    /// The <see cref="UserFollowingEngine" /> containing following users.
    /// </returns>
    [MakoExtensionConstructor]
    public UserFollowingEngine(MakoClient makoClient, long uid, PrivacyPolicy privacyPolicy) : base(makoClient)
    {
        makoClient.CheckPrivacyPolicy(privacyPolicy, uid);

        _uid = uid;
        _privacyPolicy = privacyPolicy;
    }

    public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = new()) =>
        new RecursivePixivAsyncEnumerators.User<UserFollowingEngine>(
            this,
            "/v1/user/following" +
            $"?user_id={_uid}" +
            $"&restrict={_privacyPolicy.GetDescription()}");
}
