// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Global.Enum;

public enum WorkType
{
    [JsonStringEnumMemberName("illust")]
    Illustration,

    [JsonStringEnumMemberName("manga")]
    Manga,

    [JsonStringEnumMemberName("novel")]
    Novel
}

public enum SimpleWorkType
{
    Illustration,

    Novel
}
