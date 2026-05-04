// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Runtime.CompilerServices;
using Mako.Engine;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako;

[GeneratedMakoExtension]
public partial class MakoClient
{
    /// <inheritdoc cref="IllustrationRecommended" />
    public IFetchEngine<IWorkEntry> WorkRecommended(
        WorkType type,
        bool includeRankingWorks = true,
        bool includePrivacyPolicy = true)
    {
        return type switch
        {
            WorkType.Novel => NovelRecommended(includeRankingWorks, includePrivacyPolicy),
            WorkType.Manga => MangaRecommended(includeRankingWorks, includePrivacyPolicy),
            _ => IllustrationRecommended(includeRankingWorks, includePrivacyPolicy)
        };
    }

    /// <inheritdoc cref="IllustrationBookmarks" />
    public IFetchEngine<IWorkEntry> WorkBookmarks(
        long uid,
        SimpleWorkType type,
        PrivacyPolicy privacyPolicy,
        string? tag)
    {
        return type is SimpleWorkType.Novel
            ? NovelBookmarks(uid, tag, privacyPolicy)
            : IllustrationBookmarks(uid, tag, privacyPolicy);
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

    public IFetchEngine<IWorkEntry> WorkNew(
        WorkType type,
        uint? maxWorkId = null)
    {
        return type is WorkType.Novel
            ? NovelNew(maxWorkId)
            : IllustrationNew(type is WorkType.Manga, maxWorkId);
    }

    /// <inheritdoc cref="IllustrationBookmarkTags" />
    public IFetchEngine<BookmarkTag> WorkBookmarkTags(
        long uid,
        SimpleWorkType type,
        PrivacyPolicy policy)
    {
        return type is SimpleWorkType.Novel
            ? NovelBookmarkTags(uid, policy)
            : IllustrationBookmarkTags(uid, policy);
    }

    /// <inheritdoc cref="IllustrationFollowing" />
    public IFetchEngine<IWorkEntry> WorkFollowing(
        SimpleWorkType type,
        PrivacyPolicy privacyPolicy)
    {
        return type is SimpleWorkType.Novel
            ? NovelFollowing(privacyPolicy)
            : IllustrationFollowing(privacyPolicy);
    }

    /// <inheritdoc cref="IllustrationPosted" />
    public IFetchEngine<IWorkEntry> WorkPosts(
        long uid,
        WorkType type)
    {
        return type is WorkType.Novel
            ? NovelPosted(uid)
            : IllustrationPosted(uid, type);
    }

    /// <inheritdoc cref="IllustrationPosted" />
    public IFetchEngine<Series> WorkSeriesWatchlist(WorkType type)
    {
        if (type is WorkType.Illustration)
            throw new ArgumentOutOfRangeException(nameof(type));
        return type is WorkType.Novel
            ? NovelSeriesWatchlist()
            : MangaSeriesWatchlist();
    }

    /// <inheritdoc cref="IllustrationComments" />
    public IFetchEngine<Comment> WorkComments(SimpleWorkType type, long workId)
    {
        return type is SimpleWorkType.Novel
            ? NovelComments(workId)
            : IllustrationComments(workId);
    }

    /// <inheritdoc cref="IllustrationCommentReplies" />
    public IFetchEngine<Comment> WorkCommentReplies(SimpleWorkType type, long commentId)
    {
        return type is SimpleWorkType.Novel
            ? NovelCommentReplies(commentId)
            : IllustrationCommentReplies(commentId);
    }

    #region Helpers

    public static DateTimeOffset RankingMaxDateTime =>
        // 榜单在日本时间（东九区）每天中午12点更新昨日榜单
        // 即在西三区每天凌晨0点更新
        DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-3)).Date - TimeSpan.FromDays(1);

    public static DateOnly RankingMaxDate => RankingMaxDateTime.ToDateOnly();

    /// <exception cref="ArgumentException"></exception>
    internal static void CheckRankingMaxDate(DateOnly dateOnly, [CallerArgumentExpression(nameof(dateOnly))] string memberName = "")
    {
        if (dateOnly > RankingMaxDate)
            throw new ArgumentException("The specified date is out of range.", memberName);
    }

    /// <exception cref="ArgumentException"></exception>
    internal void CheckPrivacyPolicy(PrivacyPolicy privacyPolicy, long uid, [CallerArgumentExpression(nameof(privacyPolicy))] string memberName = "")
    {
        if (privacyPolicy is PrivacyPolicy.Private && Me?.Id != uid)
            throw new ArgumentException("Cannot request other user's private data.", memberName);
    }

    /// <exception cref="ArgumentException"></exception>
    internal void CheckWorkSortOption(WorkSortOption workSortOption, [CallerArgumentExpression(nameof(workSortOption))] string memberName = "")
    {
        if (!(Me?.IsPremium ?? false) && workSortOption is WorkSortOption.PopularityDescending)
            throw new ArgumentException("Cannot request premium sort option.", memberName);
    }

    #endregion
}
