﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace Mako.Util
{
    [PublicAPI]
    public static class Objects
    {
        public static readonly IEqualityComparer<string> CaseIgnoredComparer = new CaseIgnoredStringComparer();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Regex ToRegex(this string str)
        {
            return new(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty(this string? str)
        {
            return !string.IsNullOrEmpty(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrBlank(this string? str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string? str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static byte[] GetBytes(this string str, Encoding? encoding = null)
        {
            return encoding?.Let(e => e!.GetBytes(str)) ?? Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(this byte[] bytes, Encoding? encoding = null)
        {
            return encoding?.Let(e => e!.GetString(bytes)) ?? Encoding.UTF8.GetString(bytes);
        }

        public static string GetString(this MemoryOwner<byte> bytes, Encoding? encoding = null)
        {
            using (bytes)
            {
                return encoding?.Let(e => e!.GetString(bytes.Span)) ?? Encoding.UTF8.GetString(bytes.Span);
            }
        }

        public static async Task<string> HashAsync<THash>(this string str) where THash : HashAlgorithm, new()
        {
            using var hasher = new THash();
            await using var memoryStream = new MemoryStream(str.GetBytes());
            var bytes = await hasher.ComputeHashAsync(memoryStream).ConfigureAwait(false);
            return bytes.Select(b => b.ToString("x2")).Aggregate(string.Concat);
        }

        public static Task<HttpResponseMessage> GetResponseHeader(this HttpClient client, string url)
        {
            return client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        }

        public static async Task<string?> ToJsonAsync<TEntity>(this TEntity? obj, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            if (obj is null) return null;
            await using var memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, obj, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option))).ConfigureAwait(false);
            return memoryStream.ToArray().GetString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ToJson(this object? obj, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            return obj?.Let(o => JsonSerializer.Serialize(o, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option))));
        }

        public static async ValueTask<TEntity?> FromJsonAsync<TEntity>(this IMemoryOwner<byte> bytes, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            using (bytes)
            {
                await using var stream = bytes.Memory.AsStream();
                return await JsonSerializer.DeserializeAsync<TEntity>(stream, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option))).ConfigureAwait(false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEntity? FromJson<TEntity>(this string str, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            return JsonSerializer.Deserialize<TEntity>(str, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddWithRegionName(
            this ObjectCache objectCache,
            string key,
            object value,
            CacheItemPolicy policy,
            string regionName)
        {
            var realKey = $"{regionName}::{key}";
            return objectCache.Add(realKey, value, policy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetWithRegionName(
            this ObjectCache objectCache,
            string key,
            string regionName)
        {
            var realKey = $"{regionName}::{key}";
            return objectCache.Get(realKey);
        }

        public static IEnumerable<JsonProperty> EnumerateObjectOrEmpty(this JsonElement? element)
        {
            return element?.EnumerateObject() as IEnumerable<JsonProperty> ?? Array.Empty<JsonProperty>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JsonElement GetProperty(this JsonProperty jsonElement, string prop)
        {
            return jsonElement.Value.GetProperty(prop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JsonElement? GetPropertyOrNull(this JsonElement element, string prop)
        {
            return element.TryGetProperty(prop, out var result) ? result : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JsonElement? GetPropertyOrNull(this JsonProperty property, string prop)
        {
            return property.Value.TryGetProperty(prop, out var result) ? result : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetPropertyString(this JsonElement jsonElement)
        {
            return jsonElement.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetPropertyString(this JsonElement jsonElement, string prop)
        {
            return jsonElement.GetProperty(prop).ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetPropertyString(this JsonProperty jsonProperty)
        {
            return jsonProperty.Value.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetPropertyString(this JsonProperty jsonProperty, string prop)
        {
            return jsonProperty.Value.GetProperty(prop).ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetPropertyLong(this JsonProperty jsonProperty, string prop)
        {
            return jsonProperty.Value.GetProperty(prop).GetInt64();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetPropertyLong(this JsonElement jsonElement, string prop)
        {
            return jsonElement.GetProperty(prop).GetInt64();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset GetPropertyDateTimeOffset(this JsonProperty jsonProperty)
        {
            return jsonProperty.Value.GetDateTimeOffset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset GetPropertyDateTimeOffset(this JsonProperty jsonProperty, string prop)
        {
            return jsonProperty.Value.GetProperty(prop).GetDateTimeOffset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime GetPropertyDateTime(this JsonProperty jsonProperty)
        {
            return jsonProperty.Value.GetDateTime();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime GetPropertyDateTime(this JsonProperty jsonProperty, string prop)
        {
            return jsonProperty.Value.GetProperty(prop).GetDateTime();
        }


        /// <summary>
        ///     Returns <see cref="Result{T}.Failure" /> if the status code does not indicating success
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <param name="exceptionSelector"></param>
        /// <returns></returns>
        public static async Task<Result<string>> GetStringResultAsync(this HttpClient httpClient, string url, Func<HttpResponseMessage, Task<Exception>>? exceptionSelector = null)
        {
            var responseMessage = await httpClient.GetAsync(url).ConfigureAwait(false);
            return !responseMessage.IsSuccessStatusCode ? Result<string>.OfFailure(exceptionSelector is { } selector ? await selector.Invoke(responseMessage).ConfigureAwait(false) : null) : Result<string>.OfSuccess(await responseMessage.Content.ReadAsStringAsync());
        }

        public static Task<TResult[]> WhenAll<TResult>(this IEnumerable<Task<TResult>> tasks)
        {
            return Task.WhenAll(tasks);
        }

        /// <summary>
        ///     Copy all the nonnull properties of <paramref name="@this" /> to the same properties of <paramref name="another" />
        /// </summary>
        /// <param name="this"></param>
        /// <param name="another"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T With<T>(this T @this, T another)
        {
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ForEach(info => info.GetValue(@this)?.Let(o => info.SetValue(another, o)));
            return another;
        }

        private class CaseIgnoredStringComparer : IEqualityComparer<string>
        {
            public bool Equals(string? x, string? y)
            {
                return x is not null && y is not null && x.EqualsIgnoreCase(y);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}