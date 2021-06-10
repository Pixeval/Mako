using System;

namespace Mako.Util
{
    internal static class Errors
    {
        public static void ThrowNetworkException(string url, int page, string? extraMsg, bool bypass)
        {
            if (extraMsg == null) throw new ArgumentNullException(nameof(extraMsg));
            throw new MakoNetworkException(url, page, bypass, extraMsg);
        }
    }
}