using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Mako
{
    [PublicAPI]
    public class MakoNetworkException : Exception
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
}