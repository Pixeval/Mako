// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mako.Engine;
using Mako.Engine.Implements;
using Mako.Global.Enum;
using Mako.Global.Exception;
using Mako.Model;
using Mako.Utilities;

namespace Mako;

public partial class MakoClient
{
    // --------------------------------------------------
    // This part contains all APIs that depend on the
    // IFetchEngine, however, the uniqueness of the inner
    // elements is not guaranteed, call Distinct() if you
    // are care about the uniqueness of the results
    // --------------------------------------------------

    /// <summary>
    /// Request bookmarked illustrations for a user.
    /// </summary>
    /// <param name="uid">User id</param>
    /// <param name="privacyPolicy">The <see cref="PrivacyPolicy" /> options targeting private or public</param>
    /// <param name="tag"></param>
    /// <param name="targetFilter">The <see cref="TargetFilter" /> options targeting android or ios</param>
    /// <returns>
    /// The <see cref="IllustrationBookmarkEngine" />> iterator containing bookmarked illustrations for the user.
    /// </returns>
    /// <exception cref="IllegalPrivatePolicyException">Requesting other user's private bookmarks will throw this exception.</exception>
    public IFetchEngine<Illustration> IllustrationBookmarks(long uid, PrivacyPolicy privacyPolicy, string? tag, TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        EnsureNotCancelled();
        CheckPrivacyPolicy(uid, privacyPolicy);

        return new IllustrationBookmarkEngine(this, uid, tag, privacyPolicy, targetFilter, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request bookmarked novels.
    /// </summary>
    /// <param name="uid">User id</param>
    /// <param name="privacyPolicy">The <see cref="PrivacyPolicy" /> options targeting private or public</param>
    /// <param name="tag"></param>
    /// <param name="targetFilter">The <see cref="TargetFilter" /> option targeting android or ios</param>
    /// <returns>
    /// The <see cref="NovelBookmarkEngine" /> containing the bookmarked novels.
    /// </returns>
    public IFetchEngine<Novel> NovelBookmarks(long uid, PrivacyPolicy privacyPolicy, string? tag, TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        EnsureNotCancelled();
        CheckPrivacyPolicy(uid, privacyPolicy);

        return new NovelBookmarkEngine(this, uid, tag, privacyPolicy, targetFilter, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Search in Pixiv.
    /// </summary>
    /// <param name="tag">Texts for searching</param>
    /// <param name="matchOption">
    /// The <see cref="SearchIllustrationTagMatchOption.TitleAndCaption" /> option for the method of search
    /// matching
    /// </param>
    /// <param name="sortOption">The <see cref="WorkSortOption" /> option for sorting method</param>
    /// <param name="targetFilter">The <see cref="TargetFilter" /> option targeting android or ios</param>
    /// <param name="startDate">The starting date filtering the search results</param>
    /// <param name="endDate">The ending date filtering the searching results</param>
    /// <param name="aiType"></param>
    /// <returns>
    /// The <see cref="IllustrationSearchEngine" /> iterator containing the searching results.
    /// </returns>
    public IFetchEngine<Illustration> SearchIllustrations(
        string tag,
        SearchIllustrationTagMatchOption matchOption = SearchIllustrationTagMatchOption.TitleAndCaption,
        WorkSortOption sortOption = WorkSortOption.DoNotSort,
        TargetFilter targetFilter = TargetFilter.ForAndroid,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        bool? aiType = null)
    {
        EnsureNotCancelled();
        if (sortOption is WorkSortOption.PopularityDescending && !Me.IsPremium)
            sortOption = WorkSortOption.DoNotSort;

        return new IllustrationSearchEngine(this, new EngineHandle(CancelInstance), matchOption, tag, sortOption, targetFilter, startDate, endDate, aiType);
    }

    public IFetchEngine<Novel> SearchNovels(
        string tag,
        SearchNovelTagMatchOption matchOption = SearchNovelTagMatchOption.Text,
        WorkSortOption sortOption = WorkSortOption.DoNotSort,
        TargetFilter targetFilter = TargetFilter.ForAndroid,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        bool mergePlainKeywordResults = true,
        bool includeTranslatedTagResults = true,
        bool? aiType = null)
    {
        EnsureNotCancelled();
        if (sortOption is WorkSortOption.PopularityDescending && !Me.IsPremium)
            sortOption = WorkSortOption.DoNotSort;

        return new NovelSearchEngine(this, new EngineHandle(CancelInstance), matchOption, tag, sortOption, targetFilter, startDate, endDate, mergePlainKeywordResults, includeTranslatedTagResults, aiType);
    }

    /// <summary>
    /// Search user in Pixiv.
    /// </summary>
    /// <param name="keyword">The text in searching</param>
    /// <param name="targetFilter">The <see cref="TargetFilter" /> option targeting android or ios</param>
    /// <returns>
    /// The <see cref="UserSearchEngine" /> containing the search results for users.
    /// </returns>
    public IFetchEngine<User> SearchUser(
        string keyword,
        TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        EnsureNotCancelled();
        return new UserSearchEngine(this, targetFilter, keyword, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request ranking illustrations in Pixiv.
    /// </summary>
    /// <param name="rankOption">The option of which the <see cref="RankOption" /> of rankings</param>
    /// <param name="dateTime">The date of rankings</param>
    /// <param name="targetFilter">The <see cref="TargetFilter" /> option targeting android or ios</param>
    /// <returns>
    /// The <see cref="IllustrationRankingEngine" /> containing rankings.
    /// </returns>
    /// <exception cref="RankingDateOutOfRangeException">
    /// Throw this exception if the date is not valid.
    /// </exception>
    public IFetchEngine<Illustration> IllustrationRanking(RankOption rankOption, DateTime dateTime, TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        EnsureNotCancelled();
        if (DateTime.Today - dateTime.Date < TimeSpan.FromDays(2))
        {
            ThrowHelper.Throw(new RankingDateOutOfRangeException());
        }

        return new IllustrationRankingEngine(this, rankOption, dateTime, targetFilter, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<Novel> NovelRanking(RankOption rankOption, DateTime dateTime, TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        EnsureNotCancelled();
        if (DateTime.Today - dateTime.Date < TimeSpan.FromDays(2))
        {
            ThrowHelper.Throw(new RankingDateOutOfRangeException());
        }

        return new NovelRankingEngine(this, rankOption, dateTime, targetFilter, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request recommended illustrations in Pixiv.
    /// </summary>
    /// <param name="recommendContentType">The <see cref="WorkType" />Option for illust or manga (not novel)</param>
    /// <param name="targetFilter">The <see cref="TargetFilter" /> option targeting android or ios</param>
    /// <param name="maxBookmarkIdForRecommend">Max bookmark id for recommendation</param>
    /// <param name="minBookmarkIdForRecentIllust">Min bookmark id for recent illust</param>
    /// <returns>
    /// The <see cref="RecommendIllustrationEngine" /> containing recommended illustrations.
    /// </returns>
    public IFetchEngine<Illustration> RecommendationIllustrations(
        TargetFilter targetFilter = TargetFilter.ForAndroid,
        WorkType? recommendContentType = null,
        uint? maxBookmarkIdForRecommend = null,
        uint? minBookmarkIdForRecentIllust = null)
    {
        EnsureNotCancelled();
        return new RecommendIllustrationEngine(this, recommendContentType, targetFilter, maxBookmarkIdForRecommend, minBookmarkIdForRecentIllust, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<Illustration> RecommendationMangas(
        TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        EnsureNotCancelled();
        return new RecommendMangaEngine(this, targetFilter, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<Novel> RecommendationNovels(
        TargetFilter targetFilter = TargetFilter.ForAndroid,
        uint? maxBookmarkIdForRecommend = null)
    {
        EnsureNotCancelled();
        return new RecommendNovelEngine(this, targetFilter, maxBookmarkIdForRecommend, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<IWorkEntry> RecommendationWorks(
        WorkType recommendContentType,
        TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        return recommendContentType switch
        {
            WorkType.Novel => RecommendationNovels(targetFilter),
            WorkType.Manga => RecommendationMangas(targetFilter),
            _ => RecommendationIllustrations(targetFilter)
        };
    }

    /// <summary>
    /// Request recommended illustrators.
    /// </summary>
    /// <param name="targetFilter">The <see cref="TargetFilter" /> option targeting android or ios</param>
    /// <returns>
    /// The <see cref="RecommendIllustratorEngine" /> containing recommended illustrators.
    /// </returns>
    public IFetchEngine<User> RecommendIllustrators(TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        EnsureNotCancelled();
        return new RecommendIllustratorEngine(this, targetFilter, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<Illustration> NewIllustrations(WorkType workType, TargetFilter targetFilter = TargetFilter.ForAndroid, uint? maxIllustId = null)
    {
        EnsureNotCancelled();
        return new IllustrationNewEngine(this, workType, targetFilter, maxIllustId, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<Novel> NewNovels(TargetFilter targetFilter = TargetFilter.ForAndroid, uint? maxNovelId = null)
    {
        EnsureNotCancelled();
        return new NovelNewEngine(this, targetFilter, maxNovelId, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<IWorkEntry> NewWorks(WorkType type,
        TargetFilter targetFilter = TargetFilter.ForAndroid,
        uint? maxId = null)
    {
        return type switch
        {
            WorkType.Novel => NewNovels(targetFilter, maxId),
            _ => NewIllustrations(type, targetFilter, maxId)
        };
    }
    public IFetchEngine<User> MyPixivUsers(long userId)
    {
        EnsureNotCancelled();
        return new MyPixivUserEngine(this, userId, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request the spotlights in Pixiv.
    /// </summary>
    /// <returns>
    /// The <see cref="SpotlightEngine" /> containing the spotlight articles.
    /// </returns>
    public IFetchEngine<Spotlight> Spotlights()
    {
        EnsureNotCancelled();
        return new SpotlightEngine(this, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request feeds (the recent activity of following users)
    /// </summary>
    /// <returns>
    /// The <see cref="FeedEngine" /> containing the feeds.
    /// </returns>
    public IFetchEngine<Feed?> Feeds()
    {
        EnsureNotCancelled();
        return new FeedEngine(this, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request following users of a user.
    /// </summary>
    /// <param name="uid">User id</param>
    /// <param name="privacyPolicy">The <see cref="PrivacyPolicy" /> options targeting private or public</param>
    /// <returns>
    /// The <see cref="FollowingEngine" /> containing following users.
    /// </returns>
    /// <exception cref="IllegalPrivatePolicyException"></exception>
    public IFetchEngine<User> Following(long uid, PrivacyPolicy privacyPolicy)
    {
        EnsureNotCancelled();
        CheckPrivacyPolicy(uid, privacyPolicy);

        return new FollowingEngine(this, uid, privacyPolicy, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<BookmarkTag> IllustrationBookmarkTag(long uid, PrivacyPolicy privacyPolicy)
    {
        EnsureNotCancelled();
        CheckPrivacyPolicy(uid, privacyPolicy);

        return new IllustrationBookmarkTagEngine(this, uid, privacyPolicy, new EngineHandle(CancelInstance));
    }

    public async Task<List<BookmarkTag>> GetBookmarkTagAsync(long uid, SimpleWorkType type, PrivacyPolicy policy)
    {
        var array = (policy, type) switch
        {
            (PrivacyPolicy.Private, SimpleWorkType.IllustAndManga) => await IllustrationBookmarkTag(uid, policy).ToListAsync(),
            (PrivacyPolicy.Public, SimpleWorkType.IllustAndManga) => await IllustrationBookmarkTag(uid, policy).ToListAsync(),
            (PrivacyPolicy.Private, SimpleWorkType.Novel) => await NovelBookmarkTag(uid, policy).ToListAsync(),
            (PrivacyPolicy.Public, SimpleWorkType.Novel) => await NovelBookmarkTag(uid, policy).ToListAsync(),
            _ => ThrowHelper.ArgumentOutOfRange<(PrivacyPolicy, SimpleWorkType), List<BookmarkTag>>((policy, type))
        };

        var allTag = new BookmarkTag
        {
            Name = BookmarkTag.AllCountedTagString,
            Count = array.Sum(t => t.Count)
        };

        array.Insert(0, allTag);

        return array;
    }

    public IFetchEngine<BookmarkTag> NovelBookmarkTag(long uid, PrivacyPolicy privacyPolicy)
    {
        EnsureNotCancelled();
        CheckPrivacyPolicy(uid, privacyPolicy);

        return new NovelBookmarkTagEngine(this, uid, privacyPolicy, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request recent posts of following users.
    /// </summary>
    /// <param name="privacyPolicy">The <see cref="PrivacyPolicy" /> options targeting private or public</param>
    /// <returns>
    /// The <see cref="RecentPostedIllustrationEngine" /> containing the recent posts.
    /// </returns>
    public IFetchEngine<Illustration> RecentIllustrationPosts(PrivacyPolicy privacyPolicy)
    {
        EnsureNotCancelled();
        return new RecentPostedIllustrationEngine(this, privacyPolicy, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<Novel> RecentNovelPosts(PrivacyPolicy privacyPolicy)
    {
        EnsureNotCancelled();
        return new RecentPostedNovelEngine(this, privacyPolicy, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request posts of a user.
    /// </summary>
    /// <param name="uid">User id.</param>
    /// <param name="recommendContentType">The <see cref="WorkType" /> option for illust or manga (not novel)</param>
    /// <param name="targetFilter">The <see cref="TargetFilter" /> option targeting android or ios</param>
    /// <returns>
    /// The <see cref="PostedIllustrationEngine" /> containing posts of that user.
    /// </returns>
    public IFetchEngine<Illustration> IllustrationPosts(long uid,
        WorkType recommendContentType = WorkType.Illust,
        TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        EnsureNotCancelled();
        return new PostedIllustrationEngine(this, uid, recommendContentType, targetFilter, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request novel posts of that user.
    /// </summary>
    /// <param name="uid">User id</param>
    /// <param name="targetFilter">The <see cref="TargetFilter" /> option targeting android or ios</param>
    /// <returns>
    /// The <see cref="PostedNovelEngine" /> containing the novel posts of that user.
    /// </returns>
    public IFetchEngine<Novel> NovelPosts(long uid, TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        EnsureNotCancelled();
        return new PostedNovelEngine(this, uid, targetFilter, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<IWorkEntry> WorkPosts(long uid,
        WorkType recommendContentType = WorkType.Illust,
        TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        return recommendContentType is WorkType.Novel
            ? NovelPosts(uid, targetFilter)
            : IllustrationPosts(uid, recommendContentType, targetFilter);
    }

    /// <summary>
    /// Request comments of an illustration.
    /// </summary>
    /// <param name="illustId">Illustration id</param>
    /// <returns>
    /// The <see cref="IllustrationCommentsEngine" /> containing comments of the illustration.
    /// </returns>
    public IFetchEngine<Comment> IllustrationComments(long illustId)
    {
        EnsureNotCancelled();
        return new IllustrationCommentsEngine(illustId, this, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request comments of an illustration.
    /// </summary>
    /// <param name="illustId">Illustration id</param>
    /// <returns>
    /// The <see cref="IllustrationCommentsEngine" /> containing comments of the illustration.
    /// </returns>
    public IFetchEngine<Comment> NovelComments(long illustId)
    {
        EnsureNotCancelled();
        return new NovelCommentsEngine(illustId, this, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request replies of a comment.
    /// </summary>
    /// <param name="commentId">Comment id</param>
    /// <returns>
    /// The <see cref="IllustrationCommentRepliesEngine" /> containing replies of the comment.
    /// </returns>
    public IFetchEngine<Comment> IllustrationCommentReplies(long commentId)
    {
        EnsureNotCancelled();
        return new IllustrationCommentRepliesEngine(commentId.ToString(), this, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request replies of a comment.
    /// </summary>
    /// <param name="commentId">Comment id</param>
    /// <returns>
    /// The <see cref="IllustrationCommentRepliesEngine" /> containing replies of the comment.
    /// </returns>
    public IFetchEngine<Comment> NovelCommentReplies(long commentId)
    {
        EnsureNotCancelled();
        return new NovelCommentRepliesEngine(commentId.ToString(), this, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<Illustration> RelatedWorks(long illustId, TargetFilter targetFilter = TargetFilter.ForAndroid)
    {
        EnsureNotCancelled();
        return new RelatedWorksFetchEngine(illustId, this, targetFilter, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<T> Computed<T>(IAsyncEnumerable<T> result)
    {
        return new ComputedFetchEngine<T>(result, this, new EngineHandle(CancelInstance));
    }
}
