// Copyright (c) Pixeval.Utilities.
// Licensed under the GPL v3 License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mako.Utilities;

public static class Enumerates
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
    {
        return enumerable.Where(i => i is not null)!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable, Func<T, object?> keySelector)
    {
        return enumerable.Where(i => i is not null && keySelector(i) is not null)!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TResult> SelectNotNull<T, TResult>(this IEnumerable<T> src, Func<T, TResult?> selector) where TResult : notnull
    {
        return src.WhereNotNull().Select(selector).WhereNotNull();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TResult> SelectNotNull<T, TResult>(this IEnumerable<T> src, Func<T, object?> keySelector, Func<T, TResult> selector) where TResult : notnull
    {
        return src.WhereNotNull(keySelector).Select(selector).WhereNotNull();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var t in enumerable)
        {
            action(t);
        }
    }

    /// <summary>
    /// https://stackoverflow.com/a/15407252/10439146 FirstOrDefault on value types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrNull<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) where T : struct
    {
        var matches = enumerable.Where(predicate).Take(1).ToArray();
        return matches.Length is 0 ? null : matches[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstOrNull<T>(this IEnumerable<T> enumerable) where T : struct
    {
        var matches = enumerable.Take(1).ToArray();
        return matches.Length is 0 ? null : matches[0];
    }

    /// <summary>
    /// Replace a collection by update transactions, best to use with ObservableCollection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dest">Collection to be updated</param>
    /// <param name="source"></param>
    public static void ReplaceByUpdate<T>(this IList<T> dest, IEnumerable<T> source)
    {
        var enumerable = source as T[] ?? source.ToArray();
        if (enumerable.Length != 0)
        {
            _ = dest.RemoveAll(x => !enumerable.Contains(x));
            enumerable.Where(x => !dest.Contains(x)).ForEach(dest.Add);
        }
        else
        {
            dest.Clear();
        }
    }

    public static void ReplaceByUpdate<T>(this ISet<T> dest, IEnumerable<T> source)
    {
        var enumerable = source as T[] ?? source.ToArray();
        if (enumerable.Length != 0)
        {
            dest.ToArray().Where(x => !enumerable.Contains(x)).ForEach(x => dest.Remove(x));
            dest.AddRange(enumerable);
        }
        else
        {
            dest.Clear();
        }
    }

    public static void AddRange<T>(this ICollection<T> dest, IEnumerable<T> source)
    {
        foreach (var t in source)
        {
            dest.Add(t);
        }
    }

    public static int RemoveAll<T>(this IList<T> list, Predicate<T> match)
    {
        var count = 0;

        for (var i = list.Count - 1; i >= 0; i--)
        {
            if (!match(list[i]))
            {
                continue;
            }

            ++count;
            list.RemoveAt(i);
        }

        return count;
    }

    public static void AddIfAbsent<T>(this ICollection<T> collection, T item, IEqualityComparer<T>? comparer = null)
    {
        if (!collection.Contains(item, comparer))
        {
            collection.Add(item);
        }
    }

    public static Task<IEnumerable<TResult>> WhereAsync<TResult>(this Task<IEnumerable<TResult>> enumerable, Func<TResult, bool> selector)
    {
        return enumerable.ContinueWith(t => t.Result.Where(selector));
    }

    public static Task<IEnumerable<TResult>> SelectAsync<T, TResult>(this Task<IEnumerable<T>> enumerable, Func<T, TResult> selector)
    {
        return enumerable.ContinueWith(t => t.Result.Select(selector));
    }
}
