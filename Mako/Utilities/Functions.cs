// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Runtime.CompilerServices;

namespace Mako.Utilities;

internal static class Functions
{
    extension<TIn>(TIn obj)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TOut Let<TOut>(Func<TIn, TOut> block)
        {
            return block(obj);
        }
    }
}
