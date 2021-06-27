using Refit;

namespace Mako.Net.Request
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    internal class RemoveBookmarkRequest
    {
        [AliasAs("illust_id")]
        public string IllustId { get; }

        public RemoveBookmarkRequest(string illustId)
        {
            IllustId = illustId;
        }
    }
}