// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Mako.Utilities;
using Misaki;

namespace Mako.Model;

[Factory]
public partial class BookmarkTag : ITag
{
    ITagCategory ITag.Category => ITagCategory.Empty;

    [JsonPropertyName("name")]
    public required string Name { get; set; } = "";

    string ITranslatedName.TranslatedName => "";

    string ITag.Description => "";

    [JsonPropertyName("count")]
    public required int Count { get; set; }

    public override string ToString() => $"{Name} ({Count})";

    public virtual bool Equals(BookmarkTag? other)
    {
        return other is not null && (ReferenceEquals(this, other) || Name == other.Name);
    }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Name.GetHashCode();
}
