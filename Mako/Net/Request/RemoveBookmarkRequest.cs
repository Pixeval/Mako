using Refit;

namespace Mako.Net.Request
{
    public record RemoveBookmarkRequest
    {
        [AliasAs("illust_id")]
        public string IllustId { get; }

        public RemoveBookmarkRequest(string illustId)
        {
            IllustId = illustId;
        }
    }
}