// Copyright (c) Mako.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Mako.Global.Enum;

/// <summary>
/// The privacy policy of Pixiv, be aware that the <see cref="Private" /> option
/// is only permitted when the ID is pointing to yourself
/// </summary>
[JsonConverter(typeof(SnakeCaseLowerEnumConverter<PrivacyPolicy>))]
public enum PrivacyPolicy
{
    [Description("public")]
    Public,

    [Description("private")]
    Private
}
