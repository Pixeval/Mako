using System;
using System.Runtime.Serialization;
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
        public string Url { get; set; }
        
        public int Pages { get; set; }
        
        public bool Bypass { get; set; }

        public MakoNetworkException(string url, int pages, bool bypass, string? extraMsg)
            : base($"Current Requesting Url: {url}. Current Page Requested: {pages}. {extraMsg} Bypassing: {bypass}")
        {
            Url = url;
            Pages = pages;
            Bypass = bypass;
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
    /// 搜索榜单时设定的日期大于等于当前日期-2天
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
}