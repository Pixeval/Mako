// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System;
using System.Diagnostics;
using Mako.Global.Enum;
using Mako.Global.Exception;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

public class SearchArgumentsBase(string searchText)
{
    public string SearchText { get; set; } = searchText;

    public WorkSortOption SortOption { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    /// <remarks>
    /// false：过滤AI
    /// </remarks>
    public bool AiType { get; set; }

    public bool MergePlainKeywordResults { get; set; } = true;

    public bool IncludeTranslatedTagResults { get; set; } = true;

    public bool IncludePotentialViolationWorks { get; set; } = false;

    internal virtual void ValidateArguments(MakoClient makoClient)
    {
        makoClient.CheckWorkSortOption(SortOption);

        var startDateOnly = StartDate?.ToJapanTime().ToDateOnly();
        var endDateOnly = EndDate?.ToJapanTime().ToDateOnly();
        var japanToday = DateTimeHelper.JapanToday;

        // startDate 和 endDate 只能同时为或不为 null，所以实际上startDateOnly > japanToday无用
        if (startDateOnly > endDateOnly || endDateOnly > japanToday || startDateOnly > japanToday)
            throw new DateOutOfRangeException();
    }
}

public class IllustrationSearchArguments(string searchText) : SearchArgumentsBase(searchText)
{
    public SearchIllustrationTagMatchOption MatchOption { get; set; }

    public SearchIllustrationContentType ContentType { get; set; } = SearchIllustrationContentType.IllustrationAndMangaAndUgoira;

    public SearchIllustrationRatioPattern RatioPattern { get; set; } = SearchIllustrationRatioPattern.All;

    /// <remarks>
    /// 包含该值
    /// </remarks>
    public int? WidthMin { get; set; }

    /// <remarks>
    /// 包含该值
    /// </remarks>
    public int? WidthMax { get; set; }

    /// <remarks>
    /// 包含该值
    /// </remarks>
    public int? HeightMin { get; set; }

    /// <remarks>
    /// 包含该值
    /// </remarks>
    public int? HeightMax { get; set; }

    /// <summary>
    /// <seealso cref="IllustrationSearchOptions"/>
    /// </summary>
    public string? Tool { get; set; }
}

public class NovelSearchArguments(string searchText) : SearchArgumentsBase(searchText)
{
    public SearchNovelTagMatchOption MatchOption { get; set; }

    /// <summary>
    /// <seealso cref="SearchOptionsLanguage"/>
    /// </summary>
    public string? LangCode { get; set; }

    public SearchNovelContentLengthOption Option { get; set; }

    public int? ContentLengthMin { get; set; }

    public int? ContentLengthMax { get; set; }

    public bool IsOriginalOnly { get; set; }

    /// <summary>
    /// <seealso cref="SearchOptionsGenre"/>，需要<see cref="IsOriginalOnly"/>为true
    /// </summary>
    public int? GenreId { get; set; }

    public bool IsReplaceableOnly { get; set; }

    /// <inheritdoc />
    internal override void ValidateArguments(MakoClient makoClient)
    {
        base.ValidateArguments(makoClient);

        if (!(makoClient.Me?.IsPremium ?? false))
        {
            if (Option is SearchNovelContentLengthOption.TextLength or SearchNovelContentLengthOption.WordCount)
                if ((ContentLengthMin, ContentLengthMax) is not
                    ((null, null)
                    or (null, 4999)
                    or (5000, 19999)
                    or (20000, 79999)
                    or (80000, null)))
                {
                    Debug.Assert(false);
                    ContentLengthMin = ContentLengthMax = null;
                }

            if (Option is SearchNovelContentLengthOption.ReadingTime)
                if ((ContentLengthMin, ContentLengthMax) is not
                    ((null, null)
                    or (null, 9)
                    or (10, 59)
                    or (60, 179)
                    or (180, null)))
                {
                    Debug.Assert(false);
                    ContentLengthMin = ContentLengthMax = null;
                }
        }
    }
}
