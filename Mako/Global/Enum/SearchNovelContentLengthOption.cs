// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.ComponentModel;

namespace Mako.Global.Enum;

public enum SearchNovelContentLengthOption
{
    None,

    /// <summary>
    /// 文字数
    /// </summary>
    /// <remarks>
    /// 非会员选项：
    /// <code>
    /// -4999 微型小说
    /// 5000-19999 短篇小说
    /// 20000-79999 中篇小说
    /// 80000- 长篇小说
    /// </code>
    /// </remarks>
    [Description("text_length")]
    TextLength,

    /// <summary>
    /// 单词数（仅字母类语言）
    /// </summary>
    /// <remarks>
    /// 非会员选项：
    /// <code>
    /// -4999
    /// 5000-19999
    /// 20000-79999
    /// 80000-
    /// </code>
    /// </remarks>
    [Description("word_count")]
    WordCount,

    /// <summary>
    /// 阅读预计用时（分钟）
    /// </summary>
    /// <remarks>
    /// 非会员选项：
    /// <code>
    /// -9
    /// 10-59
    /// 60-179
    /// 180-
    /// </code>
    /// </remarks>
    [Description("reading_time")]
    ReadingTime
}
