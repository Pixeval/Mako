// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Text.Json.Serialization;
using Mako.Utilities;
using Misaki;

namespace Mako.Model;

[Factory]
public partial record Tag : ITag
{
    ITagCategory ITag.Category => ITagCategory.Empty;

    [JsonPropertyName("name")]
    public required string Name { get; set; } = "";

    string ITag.Description => ToolTip;

    [JsonPropertyName("translated_name")]
    public required string? TranslatedName { get; set; }

    string ITag.TranslatedName => TranslatedName ?? "";

    public string ToolTip => TranslatedName ?? Name;

    /// <summary>
    /// 好像只有小说会用这个属性
    /// </summary>
    [JsonPropertyName("added_by_uploaded_user")]
    public bool AddedByUploadedUser { get; set; }
}
