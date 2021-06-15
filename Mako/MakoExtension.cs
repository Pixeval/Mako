using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mako.Model;
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
        public static bool Satisfies(this Illustration illustration, IEnumerable<string> excludeTags, IEnumerable<string> includeTags, int minBookmarks)
        {
            if (illustration.Bookmarks <= minBookmarks)
            {
                return false;
            }
            
            if (illustration.Tags is { } tags)
            {
                var tagArr = tags as Tag[] ?? tags.ToArray();
                if (excludeTags.Intersect(tagArr.Select(t => t.Name).WhereNotNull(), Objects.CaseIgnoredComparer).Any())
                {
                    return false;
                }

                if (!includeTags.SequenceEquals(tagArr.Select(t => t.Name).WhereNotNull(), SequenceComparison.Unordered, Objects.CaseIgnoredComparer))
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
                IsLiked = illust.IsBookmarked,
                IsManga = illust.PageCount != 1,
                IsUgoira = illust.Type == "ugoira",
                OriginalUrl = illust.MetaSinglePage?.OriginalImageUrl,
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
    }
}