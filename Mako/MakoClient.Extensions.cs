using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mako.Model;
using Mako.Net;
using Mako.Net.Protocol;
using Mako.Net.Request;
using Mako.Net.Response;
using Mako.Util;

namespace Mako
{
    public partial class MakoClient
    {
        /// <summary>
        /// Gets the detail of an illustration from the illust id
        /// </summary>
        /// <param name="id">The illust id</param>
        /// <returns></returns>
        public async Task<Illustration> GetIllustrationFromIdAsync(string id)
        {
            var result = (await Resolve<IAppApiProtocol>().GetSingle(id)).Illust;
            return result!.ToIllustration(this);
        }

        public async Task<User> GetUserFromIdAsync(string id, TargetFilter targetFilter = TargetFilter.ForAndroid)
        {
            var result = await Resolve<IAppApiProtocol>().GetSingleUser(new SingleUserRequest(id, targetFilter.GetDescription()));
            var entity = result.UserEntity;
            return User.GetOrInstantiateAndConfigureUserFromCache(id, this, user =>
            {
                user.Avatar = entity?.ProfileImageUrls?.Medium;
                user.Follows = result.UserProfile?.TotalFollowUsers ?? 0;
                user.Id = entity?.Id.ToString();
                user.Introduction = entity?.Comment;
                user.IsFollowed = entity?.IsFollowed ?? false;
                user.IsPremium = result.UserProfile?.IsPremium ?? false;
                user.Name = entity?.Name;
            });
        }

        /// <summary>
        /// Sets the <see cref="Illustration.IsBookmarked"/> and sends a request to the Pixiv to add it to the bookmark
        /// </summary>
        /// <param name="illustration">The illustration which needs to be bookmarked</param>
        /// <param name="privacyPolicy">Indicates the privacy of the the illustration in the bookmark</param>
        /// <returns>A <see cref="Task"/> represents the operation</returns>
        public Task PostBookmarkAsync(Illustration illustration, PrivacyPolicy privacyPolicy)
        {
            illustration.SetBookmark();
            return PostBookmarkAsync(illustration.Id!, privacyPolicy);
        }

        /// <summary>
        /// Unsets the <see cref="Illustration.IsBookmarked"/> and sends a request to the Pixiv to remove it from the bookmark
        /// </summary>
        /// <param name="illustration">The illustration which needs to be removed from the bookmark</param>
        /// <returns>A <see cref="Task"/> represents the operation</returns>
        public Task RemoveBookmarkAsync(Illustration illustration)
        {
            illustration.UnsetBookmark();
            return RemoveBookmarkAsync(illustration.Id!);
        }

        /// <summary>
        /// Sends a request to the Pixiv to add it to the bookmark
        /// </summary>
        /// <param name="id">The ID of the illustration which needs to be bookmarked</param>
        /// <param name="privacyPolicy">Indicates the privacy of the the illustration in the bookmark</param>
        /// <returns>A <see cref="Task"/> represents the operation</returns>
        public Task PostBookmarkAsync(string id, PrivacyPolicy privacyPolicy)
        {
            return Resolve<IAppApiProtocol>().AddBookmark(new AddBookmarkRequest(privacyPolicy.GetDescription(), id));
        }

        /// <summary>
        /// Sends a request to the Pixiv to remove it from the bookmark
        /// </summary>
        /// <param name="id">The ID of the illustration which needs to be removed from the bookmark</param>
        /// <returns>A <see cref="Task"/> represents the operation</returns>
        public Task RemoveBookmarkAsync(string id)
        {
            return Resolve<IAppApiProtocol>().RemoveBookmark(new RemoveBookmarkRequest(id));
        }

        /// <summary>
        /// Gets the details of a spotlight from its ID which contains the article information, introduction and illustrations
        /// </summary>
        /// <param name="spotlightId">The ID of the spotlight</param>
        /// <returns>A <see cref="Task{TResult}"/> contains the result of the operation</returns>
        public async Task<SpotlightDetail?> GetSpotlightDetailAsync(string spotlightId)
        {
            var result = (await ResolveKeyed<HttpClient>(MakoApiKind.WebApi).GetStringResultAsync($"/ajax/showcase/article?article_id={spotlightId}",
                message => MakoNetworkException.FromHttpResponseMessage(message, Configuration.Bypass))).GetOrThrow().FromJson<PixivSpotlightDetailResponse>();
            if (result?.ResponseBody is null) return null;
            var illustrations = await (result.ResponseBody.First().Illusts?.SelectNotNull(illust => Task.Run(() => GetIllustrationFromIdAsync(illust.IllustId.ToString()))).WhenAll() ?? Task.FromResult(Array.Empty<Illustration>()));
            var entry = result.ResponseBody.First().Entry;
            return new SpotlightDetail(new SpotlightArticle
            {
                Id = long.Parse(entry?.Id ?? "0"),
                Title = entry?.Title,
                ArticleUrl = entry?.ArticleUrl,
                PublishDate = DateTimeOffset.FromUnixTimeSeconds(entry?.PublishDate ?? 0),
                Thumbnail = result.ResponseBody.First().ThumbnailUrl
            }, entry?.Intro ?? string.Empty, illustrations);
        }

        public Task PostFollowUserAsync(User user, PrivacyPolicy privacyPolicy)
        {
            return user.IsFollowed ? Task.CompletedTask : PostFollowUserAsync(user.Id!, privacyPolicy);
        }

        public Task PostFollowUserAsync(string id, PrivacyPolicy privacyPolicy)
        {
            return Resolve<IAppApiProtocol>().FollowUser(new FollowUserRequest(id, privacyPolicy.GetDescription()));
        }

        public Task RemoveFollowUserAsync(User user)
        {
            return user.IsFollowed ? RemoveFollowUserAsync(user.Id!) : Task.CompletedTask;
        }

        public Task RemoveFollowUserAsync(string id)
        {
            return Resolve<IAppApiProtocol>().RemoveFollowUser(new RemoveFollowUserRequest(id));
        }
    }
}