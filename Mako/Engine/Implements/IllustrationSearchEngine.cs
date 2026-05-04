// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Global.Exception;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class IllustrationSearchEngine : AbstractPixivFetchEngine<Illustration>
{
    private readonly string _tag;
    private readonly SearchIllustrationTagMatchOption _matchOption;
    private readonly WorkSortOption _sortOption;
    private readonly DateOnly? _startDate;
    private readonly DateOnly? _endDate;
    private readonly bool _aiType;
    private readonly SearchIllustrationContentType _contentType;
    private readonly SearchIllustrationRatioPattern _ratioPattern;
    private readonly int? _widthMin;
    private readonly int? _widthMax;
    private readonly int? _heightMin;
    private readonly int? _heightMax;
    private readonly bool _mergePlainKeywordResults;
    private readonly bool _includeTranslatedTagResults;
    private readonly bool _includePotentialViolationWorks;

    /// <summary>
    /// Search in Pixiv.
    /// </summary>
    /// <param name="makoClient"></param>
    /// <param name="matchOption"></param>
    /// <param name="tag"></param>
    /// <param name="sortOption"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="aiType">false：过滤AI</param>
    /// <param name="contentType"></param>
    /// <param name="ratioPattern"></param>
    /// <param name="widthMin">包含该值</param>
    /// <param name="widthMax">包含该值</param>
    /// <param name="heightMin">包含该值</param>
    /// <param name="heightMax">包含该值</param>
    /// <param name="mergePlainKeywordResults"></param>
    /// <param name="includeTranslatedTagResults"></param>
    /// <param name="includePotentialViolationWorks"></param>
    [MakoExtensionConstructor]
    public IllustrationSearchEngine(
        MakoClient makoClient,
        string tag,
        SearchIllustrationTagMatchOption matchOption,
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
        bool includePotentialViolationWorks = false) : base(makoClient)
    {
        makoClient.CheckWorkSortOption(sortOption);

        var startDateOnly = startDate?.ToJapanTime().ToDateOnly();
        var endDateOnly = endDate?.ToJapanTime().ToDateOnly();
        var japanToday = DateTimeHelper.JapanToday;

        // startDate 和 endDate 只能同时为或不为 null，所以实际上startDateOnly > japanToday无用
        if (startDateOnly > endDateOnly || endDateOnly > japanToday || startDateOnly > japanToday)
            throw new DateOutOfRangeException();

        _tag = tag;
        _matchOption = matchOption;
        _sortOption = sortOption;
        _startDate = startDateOnly;
        _endDate = endDateOnly;
        _aiType = aiType;
        _contentType = contentType;
        _ratioPattern = ratioPattern;
        _widthMin = widthMin;
        _widthMax = widthMax;
        _heightMin = heightMin;
        _heightMax = heightMax;
        _mergePlainKeywordResults = mergePlainKeywordResults;
        _includeTranslatedTagResults = includeTranslatedTagResults;
        _includePotentialViolationWorks = includePotentialViolationWorks;
    }

    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new RecursivePixivAsyncEnumerators.Illustration<IllustrationSearchEngine>(
            this,
            "/v1/search/illust"
            + $"?search_target={_matchOption.GetDescription()}"
            + $"&word={_tag}"
            + $"&{TargetFilterParam}"
            + $"&sort={_sortOption.GetDescription()}"
            + $"&search_ai_type={(_aiType ? 1 : 0)}"
            + $"&content_type={_contentType.GetDescription()}"
            + _ratioPattern.TryGetDescription()?.Let(d => $"&ratio_pattern={d}")
            + $"&merge_plain_keyword_results={_mergePlainKeywordResults.ToString().ToLower()}"
            + $"&include_translated_tag_results={_includeTranslatedTagResults.ToString().ToLower()}"
            + $"&include_potential_violation_works={_includePotentialViolationWorks.ToString().ToLower()}"
            + _startDate?.Let(dn => $"&start_date={dn:yyyy-MM-dd}")
            + _endDate?.Let(dn => $"&end_date={dn:yyyy-MM-dd}")
            + _widthMin?.Let(dn => $"&width_min={dn}")
            + _widthMax?.Let(dn => $"&width_max={dn}")
            + _heightMin?.Let(dn => $"&height_min={dn}")
            + _heightMax?.Let(dn => $"&height_max={dn}"));
    }
}
