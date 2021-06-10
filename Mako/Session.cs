using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mako.Util;

namespace Mako
{
    [PublicAPI]
    public record Session
    {
        /// <summary>
        /// User name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Token expiration
        /// </summary>
        public DateTime ExpireIn { get; set; }

        /// <summary>
        /// Current access token
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Current refresh token
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Avatar
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// User's registered e-mail address
        /// </summary>
        public string? MailAddress { get; set; }

        /// <summary>
        /// Account for login
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        /// Password for login
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Indicates current user is Pixiv Premium or not
        /// </summary>
        public bool IsPremium { get; set; }

        /// <summary>
        /// WebAPI cookie
        /// </summary>
        public string? Cookie { get; set; }

        /// <summary>
        /// Automatically bypass GFW or not, default is set to true.
        /// If you are currently living in China Mainland, turn it on to make sure
        /// you can use Mako without using any kind of proxy, otherwise you will
        /// need a proper proxy server to bypass the GFW
        /// </summary>
        public bool Bypass { get; set; }

        /// <summary>
        /// Mirror server's host of image downloading
        /// </summary>
        public string? MirrorHost { get; set; }

        /// <summary>
        /// The time of current session's refresh
        /// </summary>
        public DateTime TokenRefreshed { get; set; }

        /// <summary>
        /// Indicates which tags should be strictly exclude when performing a query operation
        /// </summary>
        public ISet<string> ExcludeTags { get; } = new HashSet<string>();

        /// <summary>
        /// Indicates which tags should be strictly include when performing a query operation
        /// </summary>
        public ISet<string> IncludeTags { get; } = new HashSet<string>();

        /// <summary>
        /// Any illust with less bookmarks will be filtered out
        /// </summary>
        public int MinBookmark { get; set; }

        public override string? ToString()
        {
            return this.ToJson();
        }

        public bool RefreshRequired()
        {
            return AccessToken.IsNullOrEmpty() || DateTime.Now - TokenRefreshed >= TimeSpan.FromMinutes(50);
        }
    }
}