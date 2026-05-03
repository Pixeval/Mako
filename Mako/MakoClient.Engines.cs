// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mako.Engine;
using Mako.Engine.Implements;
using Mako.Global.Enum;
using Mako.Global.Exception;
using Mako.Model;
using Mako.Utilities;

namespace Mako;

public partial class MakoClient
{
    /// <summary>
    /// Request bookmarked illustrations for a user.
    /// </summary>
    /// <param name="uid">User id</param>
    /// <param name="privacyPolicy">The <see cref="PrivacyPolicy" /> options targeting private or public</param>
    /// <param name="tag"></param>
    /// <returns>
    /// The <see cref="IllustrationBookmarkEngine" />> iterator containing bookmarked illustrations for the user.
    /// </returns>
    /// <exception cref="IllegalPrivatePolicyException">Requesting other user's private bookmarks will throw this exception.</exception>
    public IFetchEngine<Illustration> IllustrationBookmarks(
        long uid,
        PrivacyPolicy privacyPolicy,
        string? tag)
    {
        EnsureBuilt();
        CheckPrivacyPolicy(uid, privacyPolicy);

        return new IllustrationBookmarkEngine(this, uid, tag, privacyPolicy, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationBookmarks" />
    public IFetchEngine<Novel> NovelBookmarks(
        long uid,
        PrivacyPolicy privacyPolicy,
        string? tag)
    {
        EnsureBuilt();
        CheckPrivacyPolicy(uid, privacyPolicy);

        return new NovelBookmarkEngine(this, uid, tag, privacyPolicy, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationBookmarks" />
    public IFetchEngine<IWorkEntry> WorkBookmarks(
        long uid,
        SimpleWorkType type,
        PrivacyPolicy privacyPolicy,
        string? tag)
    {
        return type is SimpleWorkType.Novel
            ? NovelBookmarks(uid, privacyPolicy, tag)
            : IllustrationBookmarks(uid, privacyPolicy, tag);
    }

    /// <summary>
    /// Search in Pixiv.
    /// </summary>
    /// <exception cref="DateOutOfRangeException">
    /// </exception>
    public IFetchEngine<Illustration> SearchIllustrations(
        string tag,
        SearchIllustrationTagMatchOption illustrationMatchOption = SearchIllustrationTagMatchOption.PartialMatchForTags,
        WorkSortOption sortOption = WorkSortOption.PublishDateDescending,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        bool aiType = true,
        SearchIllustrationContentType contentType = SearchIllustrationContentType.IllustrationAndMangaAndUgoira,
        SearchIllustrationRatioPattern ratioPattern = SearchIllustrationRatioPattern.All,
        int? widthMin = null,
        int? widthMax = null,
        int? heightMin = null,
        int? heightMax = null,
        bool mergePlainKeywordResults = true,
        bool includeTranslatedTagResults = true,
        bool includePotentialViolationWorks = false)
    {
        EnsureBuilt();

        if (!(Me?.IsPremium ?? false) && sortOption is WorkSortOption.PopularityDescending)
        {
            Debug.Assert(false);
            sortOption = WorkSortOption.PublishDateDescending;
        }

        var startDateOnly = startDate?.ToJapanTime().ToDateOnly();
        var endDateOnly = endDate?.ToJapanTime().ToDateOnly();
        var japanToday = DateTimeHelper.JapanToday;

        // startDate 和 endDate 只能同时为或不为 null，所以实际上startDateOnly > japanToday无用
        if (startDateOnly > endDateOnly || endDateOnly > japanToday || startDateOnly > japanToday)
            throw new DateOutOfRangeException();

        return new IllustrationSearchEngine(
            this,
            illustrationMatchOption,
            tag,
            sortOption,
            startDateOnly,
            endDateOnly,
            aiType,
            contentType,
            ratioPattern,
            widthMin,
            widthMax,
            heightMin,
            heightMax,
            mergePlainKeywordResults,
            includeTranslatedTagResults,
            includePotentialViolationWorks,
            Configuration.TargetFilter,
            new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="SearchIllustrations"/>
    public IFetchEngine<Novel> SearchNovels(
        string tag,
        SearchNovelTagMatchOption novelMatchOption = SearchNovelTagMatchOption.Text,
        WorkSortOption sortOption = WorkSortOption.PublishDateDescending,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        bool aiType = true,
        SearchOptionsLanguage? language = null,
        SearchNovelContentLengthOption option = SearchNovelContentLengthOption.None,
        int? contentLengthMin = null,
        int? contentLengthMax = null,
        bool isOriginalOnly = false,
        SearchOptionsGenre? genre = null,
        bool isReplaceableOnly = false,
        bool mergePlainKeywordResults = true,
        bool includeTranslatedTagResults = true,
        bool includePotentialViolationWorks = false)
    {
        EnsureBuilt();

        if (!(Me?.IsPremium ?? false))
        {
            if (sortOption is WorkSortOption.PopularityDescending)
            {
                Debug.Assert(false);
                sortOption = WorkSortOption.PublishDateDescending;
            }

            if (option is SearchNovelContentLengthOption.TextLength or SearchNovelContentLengthOption.WordCount)
                if ((contentLengthMin, contentLengthMax) is not
                    ((null, null)
                    or (null, 4999)
                    or (5000, 19999)
                    or (20000, 79999)
                    or (80000, null)))
                {
                    Debug.Assert(false);
                    contentLengthMin = contentLengthMax = null;
                }

            if (option is SearchNovelContentLengthOption.ReadingTime)
                if ((contentLengthMin, contentLengthMax) is not
                    ((null, null)
                    or (null, 9)
                    or (10, 59)
                    or (60, 179)
                    or (180, null)))
                {
                    Debug.Assert(false);
                    contentLengthMin = contentLengthMax = null;
                }
        }

        var startDateOnly = startDate?.ToJapanTime().ToDateOnly();
        var endDateOnly = endDate?.ToJapanTime().ToDateOnly();
        var japanToday = DateTimeHelper.JapanToday;

        // startDate 和 endDate 只能同时为或不为 null，所以实际上startDateOnly > japanToday无用
        if (startDateOnly > endDateOnly || endDateOnly > japanToday || startDateOnly > japanToday)
            throw new DateOutOfRangeException();

        return new NovelSearchEngine(
            this,
            novelMatchOption,
            tag,
            sortOption,
            startDateOnly,
            endDateOnly,
            aiType,
            language?.Code,
            option,
            contentLengthMin,
            contentLengthMax,
            isOriginalOnly,
            genre?.Id,
            isReplaceableOnly,
            mergePlainKeywordResults,
            includeTranslatedTagResults,
            includePotentialViolationWorks,
            Configuration.TargetFilter,
            new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Search user in Pixiv.
    /// </summary>
    /// <param name="keyword">The text in searching</param>
    /// <returns>
    /// The <see cref="IFetchEngine{T}" /> containing the search results for users.
    /// </returns>
    public IFetchEngine<User> SearchUser(string keyword)
    {
        EnsureBuilt();
        return new UserSearchEngine(this, Configuration.TargetFilter, keyword, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request ranking in Pixiv.
    /// </summary>
    /// <param name="rankOption">The option of which the <see cref="RankOption" /> of rankings</param>
    /// <param name="dateTime">The date of rankings</param>
    /// <returns>
    /// The <see cref="IFetchEngine{T}" /> containing rankings.
    /// </returns>
    /// <exception cref="DateOutOfRangeException">
    /// Throw this exception if the date is not valid.
    /// </exception>
    public IFetchEngine<Illustration> IllustrationRanking(
        RankOption rankOption,
        DateTimeOffset dateTime)
    {
        EnsureBuilt();
        if (!RankOption.IsIllustrationSupport(rankOption))
            throw new ArgumentOutOfRangeException(nameof(rankOption));
        var dateOnly = dateTime.ToJapanTime().ToDateOnly();
        if (RankingMaxDate.ToDateOnly() < dateOnly)
            throw new DateOutOfRangeException();

        return new IllustrationRankingEngine(this, rankOption, dateOnly, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationRanking" />
    public IFetchEngine<Novel> NovelRanking(
        RankOption rankOption,
        DateTimeOffset dateTime)
    {
        EnsureBuilt();
        if (!RankOption.IsNovelSupport(rankOption))
            throw new ArgumentOutOfRangeException(nameof(rankOption));
        var dateOnly = dateTime.ToJapanTime().ToDateOnly();
        if (RankingMaxDate.ToDateOnly() < dateOnly)
            throw new DateOutOfRangeException();

        return new NovelRankingEngine(this, rankOption, dateOnly, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationRanking" />
    public IFetchEngine<IWorkEntry> WorkRanking(
        SimpleWorkType type,
        RankOption rankOption,
        DateTimeOffset dateTime)
    {
        return type is SimpleWorkType.Novel
            ? NovelRanking(rankOption, dateTime)
            : IllustrationRanking(rankOption, dateTime);
    }

    public static DateTimeOffset RankingMaxDate =>
        // 榜单在日本时间（东九区）每天中午12点更新昨日榜单
        // 即在西三区每天凌晨0点更新
        DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-3)).Date - TimeSpan.FromDays(1);

    /// <summary>
    /// Request recommended illustrations in Pixiv.
    /// </summary>
    /// <param name="type">The <see cref="WorkType" />Option for illustration or manga (not novel)</param>
    /// <param name="maxBookmarkIdForRecommend">Max bookmark id for recommendation</param>
    /// <param name="minBookmarkIdForRecentIllustration">Min bookmark id for recent illustration</param>
    /// <returns>
    /// The <see cref="RecommendedIllustrationEngine" /> containing recommended illustrations.
    /// </returns>
    public IFetchEngine<Illustration> RecommendedIllustrations(
        WorkType? type = null,
        uint? maxBookmarkIdForRecommend = null,
        uint? minBookmarkIdForRecentIllustration = null)
    {
        EnsureBuilt();
        return new RecommendedIllustrationEngine(this, type, maxBookmarkIdForRecommend, minBookmarkIdForRecentIllustration, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="RecommendedIllustrations" />
    public IFetchEngine<Illustration> RecommendedMangas()
    {
        EnsureBuilt();
        return new RecommendedMangaEngine(this, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="RecommendedIllustrations" />
    public IFetchEngine<Novel> RecommendedNovels(
        uint? maxBookmarkIdForRecommend = null)
    {
        EnsureBuilt();
        return new RecommendedNovelEngine(this, Configuration.TargetFilter, maxBookmarkIdForRecommend, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="RecommendedIllustrations" />
    public IFetchEngine<IWorkEntry> RecommendedWorks(WorkType type)
    {
        return type switch
        {
            WorkType.Novel => RecommendedNovels(),
            WorkType.Manga => RecommendedMangas(),
            _ => RecommendedIllustrations()
        };
    }

    /// <summary>
    /// Request recommended users.
    /// </summary>
    /// <returns>
    /// The <see cref="RecommendedUserEngine" /> containing recommended users.
    /// </returns>
    public IFetchEngine<User> RecommendedUsers()
    {
        EnsureBuilt();
        return new RecommendedUserEngine(this, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<Illustration> NewIllustrations(
        bool contentTypeIsManga,
        uint? maxWorkId = null)
    {
        EnsureBuilt();
        return new IllustrationNewEngine(this, contentTypeIsManga, maxWorkId, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="NewIllustrations" />
    public IFetchEngine<Novel> NewNovels(
        uint? maxWorkId = null)
    {
        EnsureBuilt();
        return new NovelNewEngine(this, maxWorkId, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="NewIllustrations" />
    public IFetchEngine<IWorkEntry> NewWorks(
        WorkType type,
        uint? maxWorkId = null)
    {
        return type is WorkType.Novel
            ? NewNovels(maxWorkId)
            : NewIllustrations(type is WorkType.Manga, maxWorkId);
    }

    public IFetchEngine<User> MyPixivUsers(long userId)
    {
        EnsureBuilt();
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
        EnsureBuilt();
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
        EnsureBuilt();
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
    public IFetchEngine<User> Following(
        long uid,
        PrivacyPolicy privacyPolicy)
    {
        EnsureBuilt();
        CheckPrivacyPolicy(uid, privacyPolicy);

        return new FollowingEngine(this, uid, privacyPolicy, new EngineHandle(CancelInstance));
    }

    public IFetchEngine<BookmarkTag> IllustrationBookmarkTag(
        long uid,
        PrivacyPolicy privacyPolicy)
    {
        EnsureBuilt();
        CheckPrivacyPolicy(uid, privacyPolicy);

        return new IllustrationBookmarkTagEngine(this, uid, privacyPolicy, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationBookmarkTag" />
    public IFetchEngine<BookmarkTag> NovelBookmarkTag(
        long uid,
        PrivacyPolicy privacyPolicy)
    {
        EnsureBuilt();
        CheckPrivacyPolicy(uid, privacyPolicy);

        return new NovelBookmarkTagEngine(this, uid, privacyPolicy, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationBookmarkTag" />
    public IFetchEngine<BookmarkTag> WorkBookmarkTag(
        long uid,
        SimpleWorkType type,
        PrivacyPolicy policy)
    {
        return type is SimpleWorkType.Novel
            ? NovelBookmarkTag(uid, policy)
            : IllustrationBookmarkTag(uid, policy);
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
        EnsureBuilt();
        return new RecentPostedIllustrationEngine(this, privacyPolicy, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="RecentIllustrationPosts" />
    public IFetchEngine<Novel> RecentNovelPosts(PrivacyPolicy privacyPolicy)
    {
        EnsureBuilt();
        return new RecentPostedNovelEngine(this, privacyPolicy, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="RecentIllustrationPosts" />
    public IFetchEngine<IWorkEntry> RecentWorkPosts(
        SimpleWorkType type,
        PrivacyPolicy privacyPolicy)
    {
        return type is SimpleWorkType.Novel
            ? RecentNovelPosts(privacyPolicy)
            : RecentIllustrationPosts(privacyPolicy);
    }

    /// <summary>
    /// Request posts of a user.
    /// </summary>
    /// <param name="uid">User id.</param>
    /// <param name="type">The <see cref="WorkType" /> option for illustration or manga (not novel)</param>
    /// <returns>
    /// The <see cref="PostedIllustrationEngine" /> containing posts of that user.
    /// </returns>
    public IFetchEngine<Illustration> IllustrationPosts(
        long uid,
        WorkType type = WorkType.Illustration)
    {
        EnsureBuilt();
        return new PostedIllustrationEngine(this, uid, type, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationPosts" />
    public IFetchEngine<Novel> NovelPosts(
        long uid)
    {
        EnsureBuilt();
        return new PostedNovelEngine(this, uid, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationPosts" />
    public IFetchEngine<IWorkEntry> WorkPosts(long uid,
        WorkType type)
    {
        return type is WorkType.Novel
            ? NovelPosts(uid)
            : IllustrationPosts(uid, type);
    }

    public IFetchEngine<Illustration> RelatedIllustrations(
        long illustrationId)
    {
        EnsureBuilt();
        return new RelatedIllustrationsFetchEngine(illustrationId, this, Configuration.TargetFilter, new EngineHandle(CancelInstance));
    }

    /// <summary>
    /// Request comments of an illustration.
    /// </summary>
    /// <param name="workId">Illustration id</param>
    /// <returns>
    /// The <see cref="IllustrationCommentsEngine" /> containing comments of the illustration.
    /// </returns>
    public IFetchEngine<Comment> IllustrationComments(long workId)
    {
        EnsureBuilt();
        return new IllustrationCommentsEngine(workId, this, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationComments" />
    public IFetchEngine<Comment> NovelComments(long workId)
    {
        EnsureBuilt();
        return new NovelCommentsEngine(workId, this, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationComments" />
    public IFetchEngine<Comment> WorkComments(SimpleWorkType type, long workId)
    {
        return type is SimpleWorkType.Novel
            ? NovelComments(workId)
            : IllustrationComments(workId);
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
        EnsureBuilt();
        return new IllustrationCommentRepliesEngine(commentId.ToString(), this, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationCommentReplies" />
    public IFetchEngine<Comment> NovelCommentReplies(long commentId)
    {
        EnsureBuilt();
        return new NovelCommentRepliesEngine(commentId.ToString(), this, new EngineHandle(CancelInstance));
    }

    /// <inheritdoc cref="IllustrationCommentReplies" />
    public IFetchEngine<Comment> WorkCommentReplies(SimpleWorkType type, long commentId)
    {
        return type is SimpleWorkType.Novel
            ? NovelCommentReplies(commentId)
            : IllustrationCommentReplies(commentId);
    }

    public IFetchEngine<T> Computed<T>(IAsyncEnumerable<T> result)
    {
        EnsureBuilt();
        return new ComputedFetchEngine<T>(result, this, new EngineHandle(CancelInstance));
    }
}
