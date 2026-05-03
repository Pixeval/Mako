// Copyright (c) Mako.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Mako.Global.Enum;

public enum SearchNovelTagMatchOption
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
    /// 正文
    /// </summary>
    [Description("text")]
    Text,

    /// <summary>
    /// 标签、标题、说明文字
    /// </summary>
    [Description("keyword")]
    Keyword
}
