// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.ComponentModel;

namespace Mako.Global.Enum;

public enum SearchIllustrationRatioPattern
{
    All,

    [Description("landscape")]
    Landscape,

    [Description("portrait")]
    Portrait,

    [Description("square")]
    Square,
}
