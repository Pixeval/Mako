using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Mako.Model
{
    [PublicAPI]
    public record Illustration
    {
        public string? Id { get; set; }

        public bool IsUgoira { get; set; }

        public string? OriginalUrl { get; set; }

        public string? LargeUrl { get; set; }

        public string? ThumbnailUrl { get; set; }

        public int Bookmarks { get; set; }

        public bool IsLiked { get; set; }

        public bool IsManga { get; set; }

        public string? Title { get; set; }

        public string? ArtistName { get; set; }

        public string? ArtistId { get; set; }

        public Illustration[]? MangaMetadata { get; set; }

        public DateTimeOffset PublishDate { get; set; }

        public int TotalViews { get; set; }

        public int TotalComments { get; set; }

        public IEnumerable<string>? Comments { get; set; }

        public Resolution? Resolution { get; set; }
        
        public IEnumerable<Tag>? Tags { get; set; }

        public bool IsR18 => Tags?.Any(x => Regex.IsMatch(x.Name ?? string.Empty, "[Rr][-]?18[Gg]?") || Regex.IsMatch(x.TranslatedName ?? string.Empty, "[Rr][-]?18[Gg]?")) ?? false;
    }
}