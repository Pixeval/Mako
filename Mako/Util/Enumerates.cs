using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Mako.Util
{
    [PublicAPI]
    public enum SequenceComparison
    {
        Sequential, Unordered
    }
    
    [PublicAPI]
    public static class Enumerates
    {
        public static bool SequenceEquals<T>(this IEnumerable<T> @this, 
            IEnumerable<T> another, 
            SequenceComparison comparison = SequenceComparison.Sequential, 
            IEqualityComparer<T>? equalityComparer = null)
        {
            return comparison switch
            {
                SequenceComparison.Sequential => @this.SequenceEqual(another, equalityComparer),
                SequenceComparison.Unordered  => @this.OrderBy(Functions.Identity<T>()).SequenceEqual(another.OrderBy(Functions.Identity<T>()), equalityComparer), // not the fastest way, but still enough
                _                             => throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null)
            };
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        {
            return enumerable.Where(i => i is not null)!;
        }

        public static IEnumerable<TResult> SelectNotNull<T, TResult>(this IEnumerable<T> src, Func<T, TResult> selector)
        {
            return src.WhereNotNull().Select(selector);
        }

        public static bool None<T>(this IEnumerable<T> enumerable) => !enumerable.Any();
    }
}