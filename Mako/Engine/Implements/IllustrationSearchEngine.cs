// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Engine.Implements;

internal class IllustrationSearchEngine : AbstractPixivFetchEngine<Illustration>
{
    private readonly IllustrationSearchArguments _arguments;

    /// <summary>
    /// Search in Pixiv.
    /// </summary>
    [MakoExtensionConstructor]
    public IllustrationSearchEngine(
        MakoClient makoClient,
        IllustrationSearchArguments arguments) : base(makoClient)
    {
        arguments.ValidateArguments(makoClient);
        _arguments = arguments;
    }

    public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        // 返回结构：{ "illusts": [...], "next_url": "...", "search_span_limit": 31536000 }
        return new RecursivePixivAsyncEnumerators.Illustration<IllustrationSearchEngine>(
            this,
            "/v1/search/illust"
            + $"?search_target={_arguments.MatchOption.GetEnumMemberName()}"
            + $"&word={_arguments.SearchText}"
            + $"&{TargetFilterParam}"
            + $"&sort={_arguments.SortOption.GetEnumMemberName()}"
            + $"&search_ai_type={(_arguments.AiType ? 1 : 0)}"
            + $"&content_type={_arguments.ContentType.GetEnumMemberName()}"
            + _arguments.RatioPattern.TryGetEnumMemberName()?.Let(d => $"&ratio_pattern={d}")
            + $"&merge_plain_keyword_results={_arguments.MergePlainKeywordResults.ToString().ToLower()}"
            + $"&include_translated_tag_results={_arguments.IncludeTranslatedTagResults.ToString().ToLower()}"
            + $"&include_potential_violation_works={_arguments.IncludePotentialViolationWorks.ToString().ToLower()}"
            + _arguments.StartDate?.Let(dn => $"&start_date={dn:yyyy-MM-dd}")
            + _arguments.EndDate?.Let(dn => $"&end_date={dn:yyyy-MM-dd}")
            + _arguments.WidthMin?.Let(dn => $"&width_min={dn}")
            + _arguments.WidthMax?.Let(dn => $"&width_max={dn}")
            + _arguments.HeightMin?.Let(dn => $"&height_min={dn}")
            + _arguments.HeightMax?.Let(dn => $"&height_max={dn}")
            + _arguments.Tool?.Let(dn => $"&tool={dn}"));
    }
}
