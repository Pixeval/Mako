// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Global.Enum;

public enum WorkSortOption
{
    [JsonStringEnumMemberName("date_desc")]
    PublishDateDescending,

    [JsonStringEnumMemberName("date_asc")]
    PublishDateAscending,

    [JsonStringEnumMemberName("popular_desc")]
    PopularityDescending
}
