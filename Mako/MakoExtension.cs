using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Protocol;
using Mako.Net.Request;
using Mako.Net.Response;
using Mako.Util;

namespace Mako
{
    [PublicAPI]
    public static class MakoExtension
    {
        /// <summary>
        /// 检测一个<see cref="Illustration"/>是否符合指定的某些条件
        /// <list type="number">
        ///         <item>
        ///         <description>检测<paramref name="item"/>是否是<c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <description>检测<paramref name="collection"/>是否不包含<paramref name="item"/></description>
        ///     </item>
        ///     <item>
        ///         <description>检查<paramref name="item"/>是否符合<see cref="Satisfies(Mako.Model.Illustration,System.Collections.Generic.IEnumerable{string},System.Collections.Generic.IEnumerable{string},int)"/></description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="collection"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool Satisfies(this Illustration? item, IEnumerable<Illustration> collection, Session session)
        {
            return item is not null && collection.All(i => i.Id != item.Id) && item.Satisfies(session.ExcludeTags, session.IncludeTags, session.MinBookmark);
        }
        
        /// <summary>
        /// 检测一个<see cref="Illustration"/>是否符合指定的某些条件
        /// <list type="number">
        ///     <item>
        ///         <description>检测是否不包含任何<paramref name="excludeTags"/>中的tag</description>
        ///     </item>
        ///     <item>
        ///         <description>检测是否包含<paramref name="includeTags"/>中的所有tag</description>
        ///     </item>
        ///     <item>
        ///         <description>检测收藏数是否大于<paramref name="minBookmarks"/></description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="illustration">作品</param>
        /// <param name="excludeTags">不能包含的tag</param>
        /// <param name="includeTags">必须包含的tag</param>
        /// <param name="minBookmarks">最低收藏数</param>
        /// <returns>作品是否符合以上条件</returns>
        public static bool Satisfies(this Illustration illustration, IEnumerable<string>? excludeTags, IEnumerable<string>? includeTags, int minBookmarks)
        {
            if (illustration.Bookmarks <= minBookmarks)
            {
                return false;
            }
            
            if (illustration.Tags is { } tags)
            {
                var tagArr = tags as Tag[] ?? tags.ToArray();
                if (excludeTags is not null && excludeTags.Intersect(tagArr.Select(t => t.Name).WhereNotNull(), Objects.CaseIgnoredComparer).Any())
                {
                    return false;
                }

                if (includeTags is not null && !includeTags.SequenceEquals(tagArr.Select(t => t.Name).WhereNotNull(), SequenceComparison.Unordered, Objects.CaseIgnoredComparer))
                {
                    return false;
                }
            }

            return illustration.Bookmarks >= minBookmarks;
        }

        public static Illustration ToIllustration(this IllustrationEssential.Illust illust)
        {
            var illustration = new Illustration
            {
                Bookmarks = illust.TotalBookmarks,
                Id = illust.Id.ToString(),
                IsBookmarked = illust.IsBookmarked,
                IsManga = illust.PageCount != 1,
                IsUgoira = illust.Type == "ugoira",
                OriginalUrl = illust.ImageUrls?.Original ?? illust.MetaSinglePage?.OriginalImageUrl,
                LargeUrl = illust.ImageUrls?.Large,
                Tags = illust.Tags?.Select(t => new Tag {Name = t.Name, TranslatedName = t.TranslatedName}),
                ThumbnailUrl = illust.ImageUrls?.Medium.IsNullOrEmpty() ?? true ? illust.ImageUrls?.SquareMedium : illust.ImageUrls.Medium,
                Title = illust.Title,
                ArtistId = illust.User?.Id.ToString(),
                ArtistName = illust.User?.Name,
                Resolution = new Resolution(illust.Width, illust.Height),
                TotalViews = illust.TotalView,
                PublishDate = illust.CreateDate
            };
            if (illustration.IsManga)
            {
                illustration.MangaMetadata = illust.MetaPages?.Select(mp => illustration with
                {
                    ThumbnailUrl = mp.ImageUrls?.Medium,
                    OriginalUrl = mp.ImageUrls?.Original,
                    LargeUrl = mp.ImageUrls?.Large
                }).ToArray();

                if (!illustration.MangaMetadata?.Any() ?? true)
                {
                    throw new MangaPagesNotFoundException(illustration);
                }
                
                foreach (var i in illustration.MangaMetadata!)
                {
                    i.MangaMetadata = illustration.MangaMetadata;
                }
            }

            return illustration;
        }

        public static User ToUserIncomplete(this UserEssential.User user)
        {
            return new()
            {
                Avatar = user.UserInfo?.ProfileImageUrls?.Medium,
                Id = user.UserInfo?.Id.ToString(),
                Name = user.UserInfo?.Name,
                Thumbnails = user.Illusts?.Take(5).SelectNotNull(ToIllustration)
            };
        }
        
        public static Session ToSession(this TokenResponse tokenResponse, string password)
        {
            return Session.Default with
            {
                AccessToken = tokenResponse.AccessToken,
                Account = tokenResponse.User?.Account,
                AvatarUrl = tokenResponse.User?.ProfileImageUrls?.Px170X170,
                ExpireIn = DateTime.Now + TimeSpan.FromSeconds(tokenResponse.ExpiresIn) - TimeSpan.FromMinutes(5), // 减去5分钟是考虑到网络延迟会导致精确时间不可能恰好是一小时(TokenResponse的ExpireIn是60分钟)
                Id = tokenResponse.User?.Id,
                IsPremium = tokenResponse.User?.IsPremium ?? false,
                RefreshToken = tokenResponse.RefreshToken,
                Password = password,
                Name = tokenResponse.User?.Name
            };
        }
        
        public static Session ComposeSession(this Session oldSession, Session newSession)
        {
            return oldSession with
            {
                AccessToken = newSession.AccessToken,
                Account = newSession.Account,
                AvatarUrl = newSession.AvatarUrl,
                ExpireIn = newSession.ExpireIn,
                Id = newSession.Id,
                IsPremium = newSession.IsPremium,
                RefreshToken = newSession.RefreshToken,
                Password = newSession.Password,
                Name = newSession.Name
            };
        }

        public static Session ToSession(this TokenResponse tokenResponse, string password, string cookie)
        {
            return tokenResponse.ToSession(password) with
            {
                Cookie = cookie
            };
        }

        #region MakoClient Extensions

#pragma warning disable CA2208 // Instantiate argument exceptions correctly
        public static async Task<Illustration> GetIllustrationFromIdAsync(this MakoClient makoClient, string id)
        {
            var result = await makoClient.ResolveKeyed<HttpClient>(MakoApiKind.AppApi).GetStringResultAsync($"/v1/illust/detail&illust_id={id}",
                message => MakoNetworkException.FromHttpResponseMessage(message, makoClient.Session.Bypass));
            var response = result switch
            {
                Result<string>.Success (var content) => content.FromJson<PixivSingleIllustResponse>()!.Illust,
                Result<string>.Failure (var cause)   => throw cause!,
                _                                    => throw new ArgumentOutOfRangeException(nameof(result), result, null)
            };
            return response.ToIllustration();
        }
#pragma warning restore CA2208
        
        public static Task PostBookmarkAsync(this MakoClient makoClient, Illustration illustration, PrivacyPolicy privacyPolicy)
        {
            illustration.SetBookmark();
            return makoClient.Resolve<IAppApiProtocol>().AddBookmark(new AddBookmarkRequest(privacyPolicy.GetDescription(), illustration.Id!));
        }

        public static Task RemoveBookmarkAsync(this MakoClient makoClient, Illustration illustration)
        {
            illustration.UnsetBookmark();
            return makoClient.Resolve<IAppApiProtocol>().RemoveBookmark(new RemoveBookmarkRequest(illustration.Id!));
        }

#pragma warning disable CA2208 // Instantiate argument exceptions correctly
        public static async Task<Illustration[]> GetSpotlightIllustrationsAsync(this MakoClient makoClient, int spotlightId)
        {
            var result = await makoClient.ResolveKeyed<HttpClient>(MakoApiKind.WebApi).GetStringResultAsync($"/ajax/showcase/article?article_id={spotlightId}",
                message => MakoNetworkException.FromHttpResponseMessage(message, makoClient.Session.Bypass));
            return result switch
            {
                Result<string>.Success (var content) => await (content.FromJson<PixivSpotlightDetailResponse>().Let(
                    response => response?.ResponseBody?.First().Illusts?.SelectNotNull(illust => Task.Run(() => GetIllustrationFromIdAsync(makoClient, illust.IllustId.ToString()))))?.WhenAll() ?? Task.FromResult(Array.Empty<Illustration>())),
                Result<string>.Failure (var cause) => throw cause!,
                _                                  => throw new ArgumentOutOfRangeException(nameof(result), result, null)
            };
        }
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        #endregion
    }
}