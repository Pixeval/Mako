// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mako.Utilities;

public static class Enumerates
{
    extension<T>(IEnumerable<T?> enumerable)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> WhereNotNull()
        {
            return enumerable.Where(i => i is not null)!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> WhereNotNull(Func<T, object?> keySelector)
        {
            return enumerable.Where(i => i is not null && keySelector(i) is not null)!;
        }
    }

    extension<T>(IEnumerable<T> src)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TResult> SelectNotNull<TResult>(Func<T, TResult?> selector) where TResult : notnull
        {
            return src.WhereNotNull().Select(selector).WhereNotNull();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TResult> SelectNotNull<TResult>(Func<T, object?> keySelector, Func<T, TResult> selector) where TResult : notnull
        {
            return src.WhereNotNull(keySelector).Select(selector).WhereNotNull();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(Action<T> action)
        {
            foreach (var t in src)
            {
                action(t);
            }
        }
    }

    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    extension<T>(IEnumerable<T> enumerable) where T : struct
    {
        /// <summary>
        /// https://stackoverflow.com/a/15407252/10439146 FirstOrDefault on value types
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? FirstOrNull(Func<T, bool> predicate)
        {
            var matches = enumerable.Where(predicate).Take(1).ToArray();
            return matches.Length is 0 ? null : matches[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? FirstOrNull()
        {
            var matches = enumerable.Take(1).ToArray();
            return matches.Length is 0 ? null : matches[0];
        }
    }

    public static void AddRange<T>(this ICollection<T> dest, IEnumerable<T> source)
    {
        foreach (var t in source)
        {
            dest.Add(t);
        }
    }
}
