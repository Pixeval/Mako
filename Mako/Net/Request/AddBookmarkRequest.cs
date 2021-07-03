using Refit;

namespace Mako.Net.Request
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable MemberCanBePrivate.Global
    internal class AddBookmarkRequest
    {
        public AddBookmarkRequest(string restrict, string id)
        {
            Restrict = restrict;
            Id = id;
        }

        [AliasAs("restrict")]
        public string Restrict { get; }

        [AliasAs("illust_id")]
        public string Id { get; }
    }
}