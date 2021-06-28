using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Preference;
using Mako.Util;

namespace Mako
{
    [PublicAPI]
    public static class MakoHelpers
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
        /// <param name="makoClient"></param>
        /// <returns></returns>
        public static bool Satisfies(this Illustration? item, IEnumerable<Illustration> collection, MakoClient makoClient)
        {
            return item is not null && collection.All(i => i.Id != item.Id) && item.Satisfies(makoClient.Configuration.ExcludeTags, makoClient.Configuration.IncludeTags, makoClient.Configuration.MinBookmark);
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

        internal static Illustration ToIllustration(this IllustrationEssential.Illust illust, MakoClient cacheProvider)
        {
            var illustration = Illustration.GetOrInstantiateAndConfigureIllustrationFromCache(illust.Id.ToString(), cacheProvider, i =>
            {
                i.Bookmarks = illust.TotalBookmarks;
                i.Id = illust.Id.ToString();
                i.IsBookmarked = illust.IsBookmarked;
                i.IsManga = illust.PageCount != 1;
                i.IsUgoira = illust.Type == "ugoira";
                i.OriginalUrl = illust.ImageUrls?.Original ?? illust.MetaSinglePage?.OriginalImageUrl;
                i.LargeUrl = illust.ImageUrls?.Large;
                i.Tags = illust.Tags?.Select(t => new Tag {Name = t.Name, TranslatedName = t.TranslatedName});
                i.ThumbnailUrl = illust.ImageUrls?.Medium.IsNullOrEmpty() ?? true ? illust.ImageUrls?.SquareMedium : illust.ImageUrls.Medium;
                i.Title = illust.Title;
                i.ArtistId = illust.User?.Id.ToString();
                i.ArtistName = illust.User?.Name;
                i.Resolution = new Resolution(illust.Width, illust.Height);
                i.TotalViews = illust.TotalView;
                i.PublishDate = illust.CreateDate;
            });
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

        public static Session ToSession(this TokenResponse tokenResponse)
        {
            return new()
            {
                AccessToken = tokenResponse.AccessToken,
                Account = tokenResponse.User?.Account,
                AvatarUrl = tokenResponse.User?.ProfileImageUrls?.Px170X170,
                ExpireIn = DateTime.Now + TimeSpan.FromSeconds(tokenResponse.ExpiresIn) - TimeSpan.FromMinutes(5), // 减去5分钟是考虑到网络延迟会导致精确时间不可能恰好是一小时(TokenResponse的ExpireIn是60分钟)
                Id = tokenResponse.User?.Id,
                IsPremium = tokenResponse.User?.IsPremium ?? false,
                RefreshToken = tokenResponse.RefreshToken,
                Name = tokenResponse.User?.Name
            };
        }
    }
}