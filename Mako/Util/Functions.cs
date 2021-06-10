using System;
using JetBrains.Annotations;

namespace Mako.Util
{
    [PublicAPI]
    public static class Functions
    {
        public static Func<T, T> Identity<T>() => t => t;

        public static ROut? Let<TIn, ROut>(this TIn obj, Func<TIn?, ROut> block)
        {
            return obj is not null ? block(obj) : default;
        }
        
        public static void Let<TIn>(this TIn obj, Action<TIn?> block)
        {
            if (obj is not null)
            {
                block(obj);
            }
        }
    }
}