// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <summary>
/// Request posts of a user.
/// </summary>
/// <param name="uid">User id.</param>
/// <param name="type">The <see cref="WorkType" /> option for illustration or manga (not novel)</param>
/// <returns>
/// The <see cref="IllustrationPostedEngine" /> containing posts of that user.
/// </returns>
[method: MakoExtensionConstructor]
internal class IllustrationPostedEngine(MakoClient makoClient, long uid, WorkType type)
    : AbstractPixivFetchEngine<Illustration>(makoClient)
{
    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new RecursivePixivAsyncEnumerators.Illustration<IllustrationPostedEngine>(
            this,
            "/v1/user/illusts"
            + $"?user_id={uid}"
            + $"&{TargetFilterParam}"
            + $"&type={type.GetDescription()}");
}
