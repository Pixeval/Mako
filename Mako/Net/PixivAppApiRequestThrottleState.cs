// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Threading;

namespace Mako.Net;

internal sealed class PixivAppApiRequestThrottleState : IDisposable
{
    public DateTimeOffset CooldownUntil { get; private set; } = DateTimeOffset.MinValue;

    public SemaphoreSlim CooldownLock { get; } = new(1, 1);

    public void ExtendCooldown(DateTimeOffset cooldownUntil)
    {
        if (cooldownUntil > CooldownUntil)
            CooldownUntil = cooldownUntil;
    }

    public void Dispose() => CooldownLock.Dispose();
}
