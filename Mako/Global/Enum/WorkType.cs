// Copyright (c) Mako.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Mako.Global.Enum;

public enum WorkType
{
    [Description("illust")]
    Illust,

    [Description("manga")]
    Manga,

    [Description("novel")]
    Novel
}

public enum SimpleWorkType
{
    IllustAndManga,

    Novel
}
