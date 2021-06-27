using Refit;

namespace Mako.Net.Request
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable MemberCanBePrivate.Global
    internal class RemoveFollowUserRequest
    {
        [AliasAs("user_id")]
        public string UserId { get; }

        public RemoveFollowUserRequest(string userId)
        {
            UserId = userId;
        }
    }
}