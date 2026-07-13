// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Global.Enum;

[JsonConverter(typeof(SnakeCaseLowerEnumConverter<TargetFilter>))]
public enum TargetFilter
{
    [JsonStringEnumMemberName("for_android")]
    ForAndroid,

    [JsonStringEnumMemberName("for_ios")]
    ForIos
}
