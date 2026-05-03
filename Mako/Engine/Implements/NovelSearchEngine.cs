// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using Mako.Global.Enum;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class NovelSearchEngine(
    MakoClient makoClient,
    SearchNovelTagMatchOption matchOption,
    string tag,
    WorkSortOption sortOption,
    DateOnly? startDate,
    DateOnly? endDate,
    bool aiType /*= true*/,
    string? langCode,
    SearchNovelContentLengthOption option,
    int? contentLengthMin,
    int? contentLengthMax,
    bool isOriginalOnly /*= false*/,
    int? genreId,
    bool isReplaceableOnly /*= false*/,
    bool mergePlainKeywordResults /*= true*/,
    bool includeTranslatedTagResults /*= true*/,
    bool includePotentialViolationWorks /*= false*/,
    TargetFilter targetFilter,
    EngineHandle? engineHandle)
    : AbstractPixivFetchEngine<Novel>(makoClient, engineHandle)
{
    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new RecursivePixivAsyncEnumerators.Novel<NovelSearchEngine>(
            this,
            "/v1/search/novel"
            + $"?search_target={matchOption.GetDescription()}"
            + $"&word={tag}"
            + $"&filter={targetFilter.GetDescription()}"
            + $"&sort={sortOption.GetDescription()}"
            + $"&search_ai_type={(aiType ? 1 : 0)}"
            + langCode?.Let(t => $"&lang={t}")
            + option.TryGetDescription()?.Let(o =>
                contentLengthMin?.Let(t => $"&{o}_min={t}")
                + contentLengthMax?.Let(t => $"&{o}_max={t}"))
            + $"&is_original_only={isOriginalOnly.ToString().ToLower()}"
            + genreId?.Let(t => $"&genre={t}")
            + $"&is_replaceable_only={isReplaceableOnly.ToString().ToLower()}"
            + $"&merge_plain_keyword_results={mergePlainKeywordResults.ToString().ToLower()}"
            + $"&include_translated_tag_results={includeTranslatedTagResults.ToString().ToLower()}"
            + $"&include_potential_violation_works={includePotentialViolationWorks.ToString().ToLower()}"
            + startDate?.Let(dn => $"&start_date={dn:yyyy-MM-dd}")
            + endDate?.Let(dn => $"&end_date={dn:yyyy-MM-dd}"));
    }
}
