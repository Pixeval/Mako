﻿#region Copyright (c) Pixeval/Mako

// MIT License
// 
// Copyright (c) Pixeval 2021 Mako/MakoClient.Engines.cs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using Mako.Engine;
using Mako.Engine.Implements;
using Mako.Global.Enum;
using Mako.Global.Exception;
using Mako.Model;
using Mako.Net.Response;
using Mako.Util;

namespace Mako
{
    public partial class MakoClient
    {
        // --------------------------------------------------
        // This part contains all APIs that depend on the
        // IFetchEngine, however, the uniqueness of the inner
        // elements is not guaranteed, call Distinct() if you
        // are care about the uniqueness of the results
        // --------------------------------------------------

        /// <summary>
        ///     Request bookmarked illustrations for a user.
        /// </summary>
        /// <param name="uid">User id</param>
        /// <param name="privacyPolicy">The <see cref="PrivacyPolicy" /> options targeting private or public</param>
        /// <param name="targetFilter">The <see cref="TargetFilter" /> options targeting android or ios</param>
        /// <returns>
        ///     The <see cref="BookmarkEngine"/>> iterator containing bookmarked illustrations for the user.
        /// </returns>
        /// <exception cref="IllegalPrivatePolicyException">Requesting other user's private bookmarks will throw this exception.</exception>
        public IFetchEngine<Illustration> Bookmarks(string uid, PrivacyPolicy privacyPolicy, TargetFilter targetFilter = TargetFilter.ForAndroid)
        {
            EnsureNotCancelled();
            if (!CheckPrivacyPolicy(uid, privacyPolicy))
            {
                throw new IllegalPrivatePolicyException(uid);
            }

            return new BookmarkEngine(this, uid, privacyPolicy, targetFilter, new EngineHandle(CancelInstance)).Apply(RegisterInstance);
        }

        /// <summary>
        ///     Search in Pixiv. 
        /// </summary>
        /// <param name="tag">Texts for searching</param>
        /// <param name="start">Start page</param>
        /// <param name="pages">Number of pages</param>
        /// <param name="matchOption">The <see cref="SearchTagMatchOption.TitleAndCaption"/> option for the method of search matching</param>
        /// <param name="sortOption">The <see cref="IllustrationSortOption"/> option for sorting method</param>
        /// <param name="searchDuration">The <see cref="SearchDuration"/> option for the duration of this search</param>
        /// <param name="targetFilter">The <see cref="TargetFilter"/> option targeting android or ios</param>
        /// <param name="startDate">The starting date filtering the search results</param>
        /// <param name="endDate">The ending date filtering the searching results</param>
        /// <returns>
        ///     The <see cref="SearchEngine"/> iterator containing the searching results.
        /// </returns>
        public IFetchEngine<Illustration> Search(
            string tag,
            int start = 0,
            int pages = 100,
            SearchTagMatchOption matchOption = SearchTagMatchOption.TitleAndCaption,
            IllustrationSortOption? sortOption = null,
            SearchDuration? searchDuration = null,
            TargetFilter? targetFilter = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            EnsureNotCancelled();
            if (sortOption == IllustrationSortOption.PopularityDescending && !Session.IsPremium)
            {
                sortOption = IllustrationSortOption.DoNotSort;
            }

            return new SearchEngine(this, new EngineHandle(CancelInstance), matchOption, tag, start, pages, sortOption, searchDuration, startDate, endDate, targetFilter);
        }

        /// <summary>
        ///     Request ranking illustrations in Pixiv.
        /// </summary>
        /// <param name="rankOption">The option of which the <see cref="RankOption"/> of rankings</param>
        /// <param name="dateTime">The date of rankings</param>
        /// <param name="targetFilter">The <see cref="TargetFilter"/> option targeting android or ios</param>
        /// <returns>
        ///     The <see cref="RankingEngine"/> containing rankings.
        /// </returns>
        /// <exception cref="RankingDateOutOfRangeException">
        ///     Throw this exception if the date is not valid.
        /// </exception>
        public IFetchEngine<Illustration> Ranking(RankOption rankOption, DateTime dateTime, TargetFilter targetFilter = TargetFilter.ForAndroid)
        {
            EnsureNotCancelled();
            if (DateTime.Today - dateTime.Date > TimeSpan.FromDays(2))
            {
                throw new RankingDateOutOfRangeException();
            }

            return new RankingEngine(this, rankOption, dateTime, targetFilter, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request recommended illustrations in Pixiv.
        /// </summary>
        /// <param name="recommendContentType">The <see cref="RecommendContentType"/> option for illust or manga</param>
        /// <param name="targetFilter">The <see cref="TargetFilter"/> option targeting android or ios</param>
        /// <param name="maxBookmarkIdForRecommend">Max bookmark id for recommendation</param>
        /// <param name="minBookmarkIdForRecentIllust">Min bookmark id for recent illust</param>
        /// <returns>
        ///     The <see cref="RecommendEngine"/> containing recommended illustrations. 
        /// </returns>
        public IFetchEngine<Illustration> Recommends(
            RecommendContentType recommendContentType = RecommendContentType.Illust,
            TargetFilter targetFilter = TargetFilter.ForAndroid,
            uint? maxBookmarkIdForRecommend = null,
            uint? minBookmarkIdForRecentIllust = null)
        {
            EnsureNotCancelled();
            return new RecommendEngine(this, recommendContentType, targetFilter, maxBookmarkIdForRecommend, minBookmarkIdForRecentIllust, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request recommended illustrators. 
        /// </summary>
        /// <param name="targetFilter">The <see cref="TargetFilter"/> option targeting android or ios</param>
        /// <returns>
        ///     The <see cref="RecommendIllustratorEngine"/> containing recommended illustrators.
        /// </returns>
        public IFetchEngine<User> RecommendIllustrators(TargetFilter targetFilter = TargetFilter.ForAndroid)
        {
            EnsureNotCancelled();
            return new RecommendIllustratorEngine(this, targetFilter, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request the spotlights in Pixiv.
        /// </summary>
        /// <returns>
        ///     The <see cref="SpotlightArticleEngine"/> containing the spotlight articles.
        /// </returns>
        public IFetchEngine<SpotlightArticle> Spotlights()
        {
            EnsureNotCancelled();
            return new SpotlightArticleEngine(this, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request feeds (the recent activity of following users)
        /// </summary>
        /// <returns>
        ///     The <see cref="FeedEngine"/> containing the feeds.
        /// </returns>
        public IFetchEngine<Feed> Feeds()
        {
            EnsureNotCancelled();
            return new FeedEngine(this, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request posts of a user.
        /// </summary>
        /// <param name="uid">User id.</param>
        /// <returns>
        ///     The <see cref="PostedIllustrationEngine"/> containing posts of that user.
        /// </returns>
        public IFetchEngine<Illustration> Posts(string uid)
        {
            EnsureNotCancelled();
            return new PostedIllustrationEngine(this, uid, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request following users of a user.
        /// </summary>
        /// <param name="uid">User id</param>
        /// <param name="privacyPolicy">The <see cref="PrivacyPolicy" /> options targeting private or public</param>
        /// <returns>
        ///     The <see cref="FollowingEngine"/> containing following users.
        /// </returns>
        /// <exception cref="IllegalPrivatePolicyException"></exception>
        public IFetchEngine<User> Following(string uid, PrivacyPolicy privacyPolicy)
        {
            EnsureNotCancelled();
            if (!CheckPrivacyPolicy(uid, privacyPolicy))
            {
                throw new IllegalPrivatePolicyException(uid);
            }

            return new FollowingEngine(this, privacyPolicy, uid, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Search user in Pixiv.
        /// </summary>
        /// <param name="keyword">The text in searching</param>
        /// <param name="userSortOption">The <see cref="UserSortOption"/> enum as date ascending or descending.</param>
        /// <param name="targetFilter">The <see cref="TargetFilter"/> option targeting android or ios</param>
        /// <returns>
        ///     The <see cref="UserSearchEngine"/> containing the search results for users.
        /// </returns>
        public IFetchEngine<User> SearchUser(
            string keyword,
            UserSortOption userSortOption = UserSortOption.DateDescending,
            TargetFilter targetFilter = TargetFilter.ForAndroid)
        {
            EnsureNotCancelled();
            return new UserSearchEngine(this, targetFilter, userSortOption, keyword, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request recent posts of following users.
        /// </summary>
        /// <param name="privacyPolicy">The <see cref="PrivacyPolicy" /> options targeting private or public</param>
        /// <returns>
        ///     The <see cref="RecentPostedIllustrationEngine"/> containing the recent posts.
        /// </returns>
        public IFetchEngine<Illustration> RecentPosts(PrivacyPolicy privacyPolicy)
        {
            EnsureNotCancelled();
            return new RecentPostedIllustrationEngine(this, privacyPolicy, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     This function is intended to be cooperated with <see cref="GetUserSpecifiedBookmarkTagsAsync" />, because
        ///     it requires an untranslated tag, for example, "未分類" is the untranslated name for "uncategorized",
        ///     and the API only recognizes the former one, while the latter one is usually works as the display
        ///     name
        /// </summary>
        /// <param name="uid">User id</param>
        /// <param name="tagWithOriginalName">The untranslated name of the tag</param>
        /// <returns>
        ///     The <see cref="TaggedBookmarksIdEngine"/> containing the illustrations ID for the bookmark tag.
        /// </returns>
        public IFetchEngine<string> UserTaggedBookmarksId(string uid, string tagWithOriginalName)
        {
            EnsureNotCancelled();
            return new TaggedBookmarksIdEngine(this, new EngineHandle(CancelInstance), uid, tagWithOriginalName);
        }

        /// <summary>
        ///     Similar to <see cref="UserTaggedBookmarksId"/> but get the illustrations.
        /// </summary>
        /// <param name="uid">User id</param>
        /// <param name="tagWithOriginalName">The untranslated name of the tag</param>
        /// <returns>
        ///     The <see cref="TaggedBookmarksIdEngine"/> containing the illustrations for the bookmark tag.
        /// </returns>
        public IFetchEngine<Illustration> UserTaggedBookmarks(string uid, string tagWithOriginalName)
        {
            EnsureNotCancelled();
            return new FetchEngineSelector<string, Illustration>(new TaggedBookmarksIdEngine(this, new EngineHandle(CancelInstance), uid, tagWithOriginalName), GetIllustrationFromIdAsync);
        }

        /// <summary>
        ///     Request manga posts of that user.
        /// </summary>
        /// <param name="uid">User id</param>
        /// <param name="targetFilter">The <see cref="TargetFilter"/> option targeting android or ios</param>
        /// <returns>
        ///     The <see cref="PostedMangaEngine"/> containing the manga posts of the user.
        /// </returns>
        public IFetchEngine<Illustration> MangaPosts(string uid, TargetFilter targetFilter)
        {
            EnsureNotCancelled();
            return new PostedMangaEngine(this, uid, targetFilter, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request novel posts of that user.
        /// </summary>
        /// <param name="uid">User id</param>
        /// <param name="targetFilter">The <see cref="TargetFilter"/> option targeting android or ios</param>
        /// <returns>
        ///     The <see cref="PostedNovelEngine"/> containing the novel posts of that user.
        /// </returns>
        public IFetchEngine<Novel> NovelPosts(string uid, TargetFilter targetFilter)
        {
            EnsureNotCancelled();
            return new PostedNovelEngine(this, uid, targetFilter, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request bookmarked novels.
        /// </summary>
        /// <param name="uid">User id</param>
        /// <param name="privacyPolicy">The <see cref="PrivacyPolicy" /> options targeting private or public</param>
        /// <param name="targetFilter">The <see cref="TargetFilter"/> option targeting android or ios</param>
        /// <returns>
        ///     The <see cref="NovelBookmarkEngine"/> containing the bookmarked novels.
        /// </returns>
        public IFetchEngine<Novel> NovelBookmarks(string uid, PrivacyPolicy privacyPolicy, TargetFilter targetFilter)
        {
            EnsureNotCancelled();
            CheckPrivacyPolicy(uid, privacyPolicy);
            return new NovelBookmarkEngine(this, uid, privacyPolicy, targetFilter, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request comments of an illustration.
        /// </summary>
        /// <param name="illustId">Illustration id</param>
        /// <returns>
        ///     The <see cref="IllustrationCommentsEngine"/> containing comments of the illustration.
        /// </returns>
        public IFetchEngine<IllustrationCommentsResponse.Comment> IllustrationComments(string illustId)
        {
            EnsureNotCancelled();
            return new IllustrationCommentsEngine(illustId, this, new EngineHandle(CancelInstance));
        }

        /// <summary>
        ///     Request replies of a comment.
        /// </summary>
        /// <param name="commentId">Comment id</param>
        /// <returns>
        ///     The <see cref="IllustrationCommentRepliesEngine"/> containing replies of the comment.
        /// </returns>
        public IFetchEngine<IllustrationCommentsResponse.Comment> IllustrationCommentReplies(string commentId)
        {
            EnsureNotCancelled();
            return new IllustrationCommentRepliesEngine(commentId, this, new EngineHandle(CancelInstance));
        }
    }
}