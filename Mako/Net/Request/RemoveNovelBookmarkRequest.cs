// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Net.Request;

public record RemoveNovelBookmarkRequest([property: JsonPropertyName("novel_id")] long NovelId);
