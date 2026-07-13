// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Global.Enum;

public enum RankOption
{
    [JsonStringEnumMemberName("day")]
    Day,

    [JsonStringEnumMemberName("week")]
    Week,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [JsonStringEnumMemberName("month")]
    Month,

    [JsonStringEnumMemberName("day_male")]
    DayMale,

    [JsonStringEnumMemberName("day_female")]
    DayFemale,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [JsonStringEnumMemberName("day_manga")]
    DayManga,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [JsonStringEnumMemberName("week_manga")]
    WeekManga,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [JsonStringEnumMemberName("month_manga")]
    MonthManga,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [JsonStringEnumMemberName("week_original")]
    WeekOriginal,

    [JsonStringEnumMemberName("week_rookie")]
    WeekRookie,

    [JsonStringEnumMemberName("day_r18")]
    DayR18,

    [JsonStringEnumMemberName("day_male_r18")]
    DayMaleR18,

    [JsonStringEnumMemberName("day_female_r18")]
    DayFemaleR18,

    [JsonStringEnumMemberName("week_r18")]
    WeekR18,

    [JsonStringEnumMemberName("week_r18g")]
    WeekR18G,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [JsonStringEnumMemberName("day_ai")]
    DayAi,

    /// <summary>
    /// Novel 不支持
    /// </summary>
    [JsonStringEnumMemberName("day_r18_ai")]
    DayR18Ai,

    /// <summary>
    /// Illustration 不支持
    /// </summary>
    [JsonStringEnumMemberName("week_ai")]
    WeekAi,

    /// <summary>
    /// Illustration 不支持
    /// </summary>
    [JsonStringEnumMemberName("week_ai_r18")]
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
