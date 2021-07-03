#region Copyright (c) Pixeval/Mako

// MIT License
// 
// Copyright (c) Pixeval 2021 Mako/MakoExceptions.cs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mako.Model;

namespace Mako
{
    [PublicAPI]
    public class MakoException : Exception
    {
        public MakoException()
        {
        }

        protected MakoException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MakoException([CanBeNull] string? message) : base(message)
        {
        }

        public MakoException([CanBeNull] string? message, [CanBeNull] Exception? innerException) : base(message, innerException)
        {
        }
    }

    [PublicAPI]
    public class MakoNetworkException : MakoException
    {
        public MakoNetworkException(string url, bool bypass, string? extraMsg, int statusCode)
            : base($"Network error while requesting URL: {url}:\n {extraMsg}\n Bypassing: {bypass}\n Status code: {statusCode}")
        {
            Url = url;
            Bypass = bypass;
            StatusCode = statusCode;
        }

        public string Url { get; set; }

        public bool Bypass { get; set; }
        public int StatusCode { get; }

        // We use Task<Exception> instead of Task<MakoNetworkException> to compromise with the generic variance
        public static async Task<Exception> FromHttpResponseMessageAsync(HttpResponseMessage message, bool bypass)
        {
            return new MakoNetworkException(message.RequestMessage?.RequestUri?.ToString() ?? string.Empty, bypass, await message.Content.ReadAsStringAsync().ConfigureAwait(false), (int) message.StatusCode);
        }
    }

    [PublicAPI]
    public class MangaPagesNotFoundException : MakoException
    {
        public MangaPagesNotFoundException(Illustration illustration)
        {
            Illustration = illustration;
        }

        protected MangaPagesNotFoundException([NotNull] SerializationInfo info, StreamingContext context, Illustration illustration) : base(info, context)
        {
            Illustration = illustration;
        }

        public MangaPagesNotFoundException([CanBeNull] string? message, Illustration illustration) : base(message)
        {
            Illustration = illustration;
        }

        public MangaPagesNotFoundException([CanBeNull] string? message, [CanBeNull] Exception? innerException, Illustration illustration) : base(message, innerException)
        {
            Illustration = illustration;
        }

        public Illustration Illustration { get; }
    }

    /// <summary>
    ///     搜索榜单时设定的日期大于等于当前日期-2天
    /// </summary>
    [PublicAPI]
    public class RankingDateOutOfRangeException : MakoException
    {
        public RankingDateOutOfRangeException()
        {
        }

        protected RankingDateOutOfRangeException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RankingDateOutOfRangeException([CanBeNull] string? message) : base(message)
        {
        }

        public RankingDateOutOfRangeException([CanBeNull] string? message, [CanBeNull] Exception? innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    ///     When a <see cref="PrivacyPolicy" /> is set to <see cref="PrivacyPolicy.Private" /> while the uid is not equivalent
    ///     to the <see cref="MakoClient.Session" />
    /// </summary>
    [PublicAPI]
    public class IllegalPrivatePolicyException : MakoException
    {
        public IllegalPrivatePolicyException(string uid)
        {
            Uid = uid;
        }

        protected IllegalPrivatePolicyException([NotNull] SerializationInfo info, StreamingContext context, string uid) : base(info, context)
        {
            Uid = uid;
        }

        public IllegalPrivatePolicyException([CanBeNull] string? message, string uid) : base(message)
        {
            Uid = uid;
        }

        public IllegalPrivatePolicyException([CanBeNull] string? message, [CanBeNull] Exception? innerException, string uid) : base(message, innerException)
        {
            Uid = uid;
        }

        public string Uid { get; }
    }

    /// <summary>
    ///     Raised if you're trying to set the sort option to popular_desc without a premium access
    /// </summary>
    [PublicAPI]
    public class IllegalSortOptionException : MakoException
    {
        public IllegalSortOptionException()
        {
        }

        protected IllegalSortOptionException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IllegalSortOptionException([CanBeNull] string? message) : base(message)
        {
        }

        public IllegalSortOptionException([CanBeNull] string? message, [CanBeNull] Exception? innerException) : base(message, innerException)
        {
        }
    }
}