// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

/// <summary>
/// 
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
/// <param name="targetFilter"></param>
/// <param name="engineHandle"></param>
internal class IllustrationSearchEngine(
    MakoClient makoClient,
    SearchIllustrationTagMatchOption matchOption,
    string tag,
    WorkSortOption sortOption,
    DateOnly? startDate,
    DateOnly? endDate,
    bool aiType /*= true*/,
    SearchIllustrationContentType contentType,
    SearchIllustrationRatioPattern ratioPattern,
    int? widthMin,
    int? widthMax,
    int? heightMin,
    int? heightMax,
    bool mergePlainKeywordResults /*= true*/,
    bool includeTranslatedTagResults /*= true*/,
    bool includePotentialViolationWorks /*= false*/,
    TargetFilter targetFilter,
    EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<Illustration>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new RecursivePixivAsyncEnumerators.Illustration<IllustrationSearchEngine>(
            this,
            "/v1/search/illust"
            + $"?search_target={matchOption.GetDescription()}"
            + $"&word={tag}"
            + $"&filter={targetFilter.GetDescription()}"
            + $"&sort={sortOption.GetDescription()}"
            + $"&search_ai_type={(aiType ? 1 : 0)}"
            + $"&content_type={contentType.GetDescription()}"
            + ratioPattern.TryGetDescription()?.Let(d => $"&ratio_pattern={d}")
            + $"&merge_plain_keyword_results={mergePlainKeywordResults.ToString().ToLower()}"
            + $"&include_translated_tag_results={includeTranslatedTagResults.ToString().ToLower()}"
            + $"&include_potential_violation_works={includePotentialViolationWorks.ToString().ToLower()}"
            + startDate?.Let(dn => $"&start_date={dn:yyyy-MM-dd}")
            + endDate?.Let(dn => $"&end_date={dn:yyyy-MM-dd}")
            + widthMin?.Let(dn => $"&width_min={dn}")
            + widthMax?.Let(dn => $"&width_max={dn}")
            + heightMin?.Let(dn => $"&height_min={dn}")
            + heightMax?.Let(dn => $"&height_max={dn}"));
    }
}
