// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Threading;

namespace Mako.Net;

internal sealed class PixivApiRequestThrottleState : IDisposable
{
    public DateTime Cooldown { get; set; } = DateTime.MinValue;

    public SemaphoreSlim CooldownLock { get; } = new(1, 1);

    public void Dispose() => CooldownLock.Dispose();
}
