// Copyright (c) Mako.
// Licensed under the MIT License.

using System;

namespace Mako;

public sealed class RateLimitEventArgs(DateTimeOffset retryAt) : EventArgs
{
    public DateTimeOffset RetryAt { get; } = retryAt;
}
