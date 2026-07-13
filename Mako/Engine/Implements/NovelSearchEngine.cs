// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class NovelSearchEngine : AbstractPixivFetchEngine<Novel>
{
    private readonly NovelSearchArguments _arguments;

    /// <inheritdoc cref="IllustrationSearchEngine.IllustrationSearchEngine" />
    [MakoExtensionConstructor]
    public NovelSearchEngine(
        MakoClient makoClient,
        NovelSearchArguments arguments) : base(makoClient)
    {
        arguments.ValidateArguments(makoClient);
        _arguments = arguments;
    }

    public override IAsyncEnumerator<Novel> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        // 返回结构：{ "novels": [...], "next_url": "...", "search_span_limit": 31536000 }
        return new RecursivePixivAsyncEnumerators.Novel<NovelSearchEngine>(
            this,
            "/v1/search/novel"
            + $"?{TargetFilterParam}"
            + $"&search_target={_arguments.MatchOption.GetEnumMemberName()}"
            + $"&word={_arguments.SearchText}"
            + $"&sort={_arguments.SortOption.GetEnumMemberName()}"
            + $"&search_ai_type={(_arguments.AiType ? 1 : 0)}"
            + _arguments.LangCode?.Let(t => $"&lang={t}")
            + _arguments.Option.TryGetEnumMemberName()?.Let(o =>
                _arguments.ContentLengthMin?.Let(t => $"&{o}_min={t}")
                + _arguments.ContentLengthMax?.Let(t => $"&{o}_max={t}"))
            + $"&is_original_only={_arguments.IsOriginalOnly.ToString().ToLower()}"
            + _arguments.GenreId?.Let(t => $"&genre={t}")
            + $"&is_replaceable_only={_arguments.IsReplaceableOnly.ToString().ToLower()}"
            + $"&merge_plain_keyword_results={_arguments.MergePlainKeywordResults.ToString().ToLower()}"
            + $"&include_translated_tag_results={_arguments.IncludeTranslatedTagResults.ToString().ToLower()}"
            + $"&include_potential_violation_works={_arguments.IncludePotentialViolationWorks.ToString().ToLower()}"
            + _arguments.StartDate?.Let(dn => $"&start_date={dn:yyyy-MM-dd}")
            + _arguments.EndDate?.Let(dn => $"&end_date={dn:yyyy-MM-dd}"));
    }
}
