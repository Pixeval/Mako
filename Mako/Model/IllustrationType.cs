// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Model;

[JsonConverter(typeof(SnakeCaseLowerEnumConverter<IllustrationType>))]
public enum IllustrationType
{
    [JsonStringEnumMemberName("illust")]
    Illustration,
    Manga,
    Ugoira
}
