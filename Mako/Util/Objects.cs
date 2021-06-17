﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Regex ToRegex(this string str) => new Regex(str);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty(this string? str) => !string.IsNullOrEmpty(str);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrBlank(this string? str) => !string.IsNullOrWhiteSpace(str);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// 根据指定的<see cref="Encoding"/>获取<paramref name="str"/>的字节数组形式，如果未指定
        /// <paramref name="encoding"/>参数则默认使用<see cref="Encoding.UTF8"/>
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns>字符串的字节数组</returns>
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

        /// <summary>
        /// 使用指定的Hash算法计算<paramref name="str"/>的Hash值
        /// </summary>
        /// <param name="str">要被计算的字符串</param>
        /// <typeparam name="THash">指定的Hash算法类型</typeparam>
        /// <returns>字符串的Hash值</returns>
        public static async Task<string> HashAsync<THash>(this string str) where THash : HashAlgorithm, new()
        {
            using var hasher = new THash();
            await using var memoryStream = new MemoryStream(str.GetBytes());
            var bytes = await hasher.ComputeHashAsync(memoryStream);
            return bytes.Select(b => b.ToString("x2")).Aggregate(string.Concat);
        }

        /// <summary>
        /// 发送一个请求并只获取响应头，抛弃响应内容
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="url">要请求的URL</param>
        /// <returns>响应</returns>
        public static Task<HttpResponseMessage> GetResponseHeader(this HttpClient client, string url)
        {
            return client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        }

        public static async Task<string?> ToJsonAsync<TEntity>(this TEntity? obj, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            if (obj is null) return null;
            await using var memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync<TEntity>(memoryStream, obj, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option)));
            return memoryStream.ToArray().GetString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ToJson(this object? obj, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            return obj?.Let(o => JsonSerializer.Serialize(o, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option))));
        }

        /// <summary>
        /// 异步的反序列化JSON，为了尽可能的减小数组分配，使用了<see cref="IMemoryOwner{T}"/>
        /// </summary>
        /// <remarks>本函数执行结束后将会释放作为参数的<see cref="IMemoryOwner{T}"/></remarks>
        /// <param name="bytes">要被反序列化的字节数组</param>
        /// <param name="serializerOptionConfigure">序列化选项配置</param>
        /// <typeparam name="TEntity">目标类型</typeparam>
        /// <returns>反序列化的对象</returns>
        public static async ValueTask<TEntity?> FromJsonAsync<TEntity>(this IMemoryOwner<byte> bytes, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            using (bytes)
            {
                await using var stream = bytes.Memory.AsStream();
                return await JsonSerializer.DeserializeAsync<TEntity>(stream, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option)));
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

        public static readonly IEqualityComparer<string> CaseIgnoredComparer = new CaseIgnoredStringComparer();

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