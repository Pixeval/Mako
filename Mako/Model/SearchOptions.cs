// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Model;

[Factory]
public partial record SearchOptions
{
    [JsonPropertyName("illust")]
    public required IllustrationSearchOptions IllustrationOptions { get; set; }

    [JsonPropertyName("novel")]
    public required NovelSearchOptions NovelOptions { get; set; }
}

public abstract record WorkSearchOptions
{
    [JsonPropertyName("bookmark_ranges")]
    public required IReadOnlyList<BookmarkRanges> BookmarkRanges { get; set; } = [];

    [JsonPropertyName("show_ai_condition")]
    public required bool ShowAiCondition { get; set; }

    [JsonPropertyName("lang")]
    public required SearchOptionsStructure<SearchOptionsLanguage> Languages { get; set; }
}

[Factory]
public partial record IllustrationSearchOptions : WorkSearchOptions
{
    [JsonPropertyName("tool")]
    public required SearchOptionsStructure<string> Tools { get; set; }
}

[Factory]
public partial record NovelSearchOptions : WorkSearchOptions
{
    [JsonPropertyName("genre")]
    public required SearchOptionsStructure<SearchOptionsGenre> Genres { get; set; }

    [JsonPropertyName("word_count_supported_languages")]
    public required string WordCountSupportedLanguages { get; set; } = "";
}

[Factory]
public partial record BookmarkRanges
{
    [JsonPropertyName("bookmark_num_min")]
    public required string BookmarkNumMin { get; set; } = "*";

    [JsonPropertyName("bookmark_num_max")]
    public required string BookmarkNumMax { get; set; } = "*";
}

[Factory]
public partial record SearchOptionsStructure<T>
{
    [JsonPropertyName("options")]
    public required IReadOnlyList<T> Options { get; set; } = [];
}

[Factory]
public partial record SearchOptionsLanguage
{
    [JsonPropertyName("code")]
    public required string Code { get; set; } = "";

    [JsonPropertyName("name")]
    public required string Name { get; set; } = "";
}

[Factory]
public partial record SearchOptionsGenre
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("label")]
    public required string Label { get; set; } = "";
}
