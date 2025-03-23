// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Text.Json.Serialization;
using Mako.Global.Enum;

namespace Mako.Net.Request;

public record AddIllustBookmarkRequest(
    [property: JsonPropertyName("restrict")] PrivacyPolicy Restrict,
    [property: JsonPropertyName("illust_id")] long Id,
    [property: JsonPropertyName("tags[]"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Tags);
