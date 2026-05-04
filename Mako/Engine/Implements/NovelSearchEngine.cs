// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class NovelSearchEngine : AbstractPixivFetchEngine<Novel>
{
    private readonly string _tag;
    private readonly SearchNovelTagMatchOption _matchOption;
    private readonly WorkSortOption _sortOption;
    private readonly DateOnly? _startDate;
    private readonly DateOnly? _endDate;
    private readonly bool _aiType;
    private readonly string? _langCode;
    private readonly SearchNovelContentLengthOption _option;
    private readonly int? _contentLengthMin;
    private readonly int? _contentLengthMax;
    private readonly bool _isOriginalOnly;
    private readonly int? _genreId;
    private readonly bool _isReplaceableOnly;
    private readonly bool _mergePlainKeywordResults;
    private readonly bool _includeTranslatedTagResults;
    private readonly bool _includePotentialViolationWorks;

    [MakoExtensionConstructor]
    public NovelSearchEngine(MakoClient makoClient,
        string tag,
        SearchNovelTagMatchOption matchOption,
        WorkSortOption sortOption = WorkSortOption.PublishDateDescending,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        bool aiType = true,
        SearchOptionsLanguage? lang = null,
        SearchNovelContentLengthOption option = SearchNovelContentLengthOption.None,
        int? contentLengthMin = null,
        int? contentLengthMax = null,
        bool isOriginalOnly = false,
        SearchOptionsGenre? genre = null,
        bool isReplaceableOnly = false,
        bool mergePlainKeywordResults = true,
        bool includeTranslatedTagResults = true,
        bool includePotentialViolationWorks = false) : base(makoClient)
    {
        makoClient.CheckWorkSortOption(sortOption);

        if (!(makoClient.Me?.IsPremium ?? false))
        {
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
            throw new ArgumentException("Invalid date range", nameof(endDate));

        _tag = tag;
        _matchOption = matchOption;
        _sortOption = sortOption;
        _startDate = startDateOnly;
        _endDate = endDateOnly;
        _aiType = aiType;
        _langCode = lang?.Code;
        _option = option;
        _contentLengthMin = contentLengthMin;
        _contentLengthMax = contentLengthMax;
        _isOriginalOnly = isOriginalOnly;
        _genreId = genre?.Id;
        _isReplaceableOnly = isReplaceableOnly;
        _mergePlainKeywordResults = mergePlainKeywordResults;
        _includeTranslatedTagResults = includeTranslatedTagResults;
        _includePotentialViolationWorks = includePotentialViolationWorks;
    }

    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new RecursivePixivAsyncEnumerators.Novel<NovelSearchEngine>(
            this,
            "/v1/search/novel"
            + $"?search_target={_matchOption.GetDescription()}"
            + $"&word={_tag}"
            + $"&{TargetFilterParam}"
            + $"&sort={_sortOption.GetDescription()}"
            + $"&search_ai_type={(_aiType ? 1 : 0)}"
            + _langCode?.Let(t => $"&lang={t}")
            + _option.TryGetDescription()?.Let(o =>
                _contentLengthMin?.Let(t => $"&{o}_min={t}")
                + _contentLengthMax?.Let(t => $"&{o}_max={t}"))
            + $"&is_original_only={_isOriginalOnly.ToString().ToLower()}"
            + _genreId?.Let(t => $"&genre={t}")
            + $"&is_replaceable_only={_isReplaceableOnly.ToString().ToLower()}"
            + $"&merge_plain_keyword_results={_mergePlainKeywordResults.ToString().ToLower()}"
            + $"&include_translated_tag_results={_includeTranslatedTagResults.ToString().ToLower()}"
            + $"&include_potential_violation_works={_includePotentialViolationWorks.ToString().ToLower()}"
            + _startDate?.Let(dn => $"&start_date={dn:yyyy-MM-dd}")
            + _endDate?.Let(dn => $"&end_date={dn:yyyy-MM-dd}"));
    }
}
