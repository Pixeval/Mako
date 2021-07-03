using Refit;

namespace Mako.Net.Request
{
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    internal class SingleUserRequest
    {
        public SingleUserRequest(string id, string filter)
        {
            Id = id;
            Filter = filter;
        }

        [AliasAs("user_id")]
        public string Id { get; }

        [AliasAs("filter")]
        public string Filter { get; }
    }
}