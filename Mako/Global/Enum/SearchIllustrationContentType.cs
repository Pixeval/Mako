// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Text.Json.Serialization;

namespace Mako.Global.Enum;

public enum SearchIllustrationContentType
{
    /// <summary>
    /// 插画、漫画、动图
    /// </summary>
    [JsonStringEnumMemberName("illust_and_manga_and_ugoira")]
    IllustrationAndMangaAndUgoira,

    /// <summary>
    /// 插画、动图
    /// </summary>
    [JsonStringEnumMemberName("illust_and_ugoira")]
    IllustrationAndUgoira,

    /// <summary>
    /// 插画
    /// </summary>
    [JsonStringEnumMemberName("illust")]
    Illustration,

    /// <summary>
    /// 漫画
    /// </summary>
    [JsonStringEnumMemberName("manga")]
    Manga,

    /// <summary>
    /// 动图
    /// </summary>
    [JsonStringEnumMemberName("ugoira")]
    Ugoira
}
