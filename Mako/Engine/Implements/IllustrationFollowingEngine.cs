// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <summary>
/// Request recent posts of following users.
/// </summary>
/// <param name="privacyPolicy">The <see cref="PrivacyPolicy" /> options targeting private or public</param>
/// <returns>
/// The <see cref="IllustrationFollowingEngine" /> containing the recent posts.
/// </returns>
[method: MakoExtensionConstructor]
internal class IllustrationFollowingEngine(MakoClient makoClient, PrivacyPolicy privacyPolicy)
    : AbstractPixivFetchEngine<Illustration>(makoClient)
{
    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<IllustrationFollowingEngine>(
            this,
            "/v2/illust/follow" +
            $"?restrict={privacyPolicy.GetDescription()}");
}
