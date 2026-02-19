// Copyright (c) Mako.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Mako.Global.Enum;

public enum RankOption
{
    [Description("day")]
    Day,

    [Description("week")]
    Week,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [Description("month")]
    Month,

    [Description("day_male")]
    DayMale,

    [Description("day_female")]
    DayFemale,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [Description("day_manga")]
    DayManga,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [Description("week_manga")]
    WeekManga,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [Description("month_manga")]
    MonthManga,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [Description("week_original")]
    WeekOriginal,

    [Description("week_rookie")]
    WeekRookie,

    [Description("day_r18")]
    DayR18,

    [Description("day_male_r18")]
    DayMaleR18,

    [Description("day_female_r18")]
    DayFemaleR18,

    [Description("week_r18")]
    WeekR18,

    [Description("week_r18g")]
    WeekR18G,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [Description("day_ai")]
    DayAi,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [Description("day_r18_ai")]
    DayR18Ai,

    /// <summary>
    /// Illustration 不支持
    /// </summary>
    [Description("week_ai")]
    WeekAi,

    /// <summary>
    /// Illustration 不支持
    /// </summary>
    [Description("week_ai_r18")]
    WeekAiR18
}

public static class RankOptionHelper
{
    extension(RankOption)
    {
        public static bool IsIllustrationSupport(RankOption rankOption) => rankOption is not RankOption.WeekAi and not RankOption.WeekAiR18;

        public static bool IsNovelSupport(RankOption rankOption) => rankOption is not RankOption.Month and not RankOption.DayManga and not RankOption.WeekManga and not RankOption.MonthManga and not RankOption.WeekOriginal and not RankOption.DayAi and not RankOption.DayR18Ai;
    }
}
