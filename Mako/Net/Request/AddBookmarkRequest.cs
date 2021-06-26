using Refit;

namespace Mako.Net.Request
{
    internal record AddBookmarkRequest
    {
        [AliasAs("restrict")]
        public string Restrict { get; }
        
        [AliasAs("illust_id")]
        public string Id { get; }

        public AddBookmarkRequest(string restrict, string id)
        {
            Restrict = restrict;
            Id = id;
        }
    }
}