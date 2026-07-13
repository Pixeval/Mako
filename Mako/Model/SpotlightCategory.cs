// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Model;

[JsonConverter(typeof(SnakeCaseLowerEnumConverter<SpotlightCategory>))]
public enum SpotlightCategory
{
    [JsonStringEnumMemberName("all")]
    All,

    [JsonStringEnumMemberName("spotlight")]
    Spotlight,

    [JsonStringEnumMemberName("tutorial")]
    Tutorial,

    [JsonStringEnumMemberName("inspiration")]
    Inspiration
}
