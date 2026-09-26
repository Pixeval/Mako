// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Threading;

namespace Mako.Net;

internal sealed class PixivAppApiRequestThrottleState : IDisposable
{
    private readonly Lock _gate = new();
    private DateTimeOffset _cooldownUntil = DateTimeOffset.MinValue;
    private DateTimeOffset _rateLimitUntil = DateTimeOffset.MinValue;

    public DateTimeOffset CooldownUntil
    {
        get
        {
            lock (_gate)
                return _cooldownUntil;
        }
    }

    public DateTimeOffset RateLimitUntil
    {
        get
        {
            lock (_gate)
                return _rateLimitUntil;
        }
    }

    public SemaphoreSlim CooldownLock { get; } = new(1, 1);

    public void ExtendCooldown(DateTimeOffset cooldownUntil)
    {
        lock (_gate)
            if (cooldownUntil > _cooldownUntil)
                _cooldownUntil = cooldownUntil;
    }

    public void ExtendRateLimit(DateTimeOffset retryAt)
    {
        lock (_gate)
        {
            if (retryAt > _rateLimitUntil)
                _rateLimitUntil = retryAt;
            if (retryAt > _cooldownUntil)
                _cooldownUntil = retryAt;
        }
    }

    public void Dispose() => CooldownLock.Dispose();
}
