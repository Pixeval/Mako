// Copyright (c) Mako.
// Licensed under the MIT License.

namespace Mako.Engine;

public interface ICancellable
{
    bool IsCancelled { get; }
}
