// Copyright (c) Mako.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Mako.Global.Enum;

public enum SearchIllustrationTagMatchOption
{
    /// <summary>
    /// 标签（部分一致）
    /// </summary>
    [Description("partial_match_for_tags")]
    PartialMatchForTags,

    /// <summary>
    /// 标签（完全一致）
    /// </summary>
    [Description("exact_match_for_tags")]
    ExactMatchForTags,

    /// <summary>
    /// 标题、说明文字
    /// </summary>
    [Description("title_and_caption")]
    TitleAndCaption,

    /// <summary>
    /// 标签、标题、说明文字
    /// </summary>
    [Description("keyword")]
    Keyword
}
