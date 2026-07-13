// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Text.Json.Serialization;

namespace Mako.Global.Enum;

public enum SearchIllustrationRatioPattern
{
    All,

    [JsonStringEnumMemberName("landscape")]
    Landscape,

    [JsonStringEnumMemberName("portrait")]
    Portrait,

    [JsonStringEnumMemberName("square")]
    Square,
}
