// Copyright (c) Mako.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Mako.Global.Enum;

[JsonConverter(typeof(SnakeCaseLowerEnumConverter<TargetFilter>))]
public enum TargetFilter
{
    [Description("for_android")]
    ForAndroid,

    [Description("for_ios")]
    ForIos
}
