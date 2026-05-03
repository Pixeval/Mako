// Copyright (c) Mako.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Mako.Global.Enum;

public enum WorkSortOption
{
    [Description("date_desc")]
    PublishDateDescending,

    [Description("date_asc")]
    PublishDateAscending,

    [Description("popular_desc")]
    PopularityDescending
}
