using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mako.Model
{
    public record Novel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("restrict")]
        public long Restrict { get; set; }

        [JsonPropertyName("x_restrict")]
        public long XRestrict { get; set; }

        [JsonPropertyName("is_original")]
        public bool IsOriginal { get; set; }

        [JsonPropertyName("image_urls")]
        public ImageUrls? Cover { get; set; }

        [JsonPropertyName("create_date")]
        public DateTimeOffset CreateDate { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag>? Tags { get; set; }

        [JsonPropertyName("page_count")]
        public long PageCount { get; set; }

        [JsonPropertyName("text_length")]
        public long TextLength { get; set; }

        [JsonPropertyName("user")]
        public UserInfo? User { get; set; }

        [JsonPropertyName("series")]
        public Series? NovelSeries { get; set; }

        [JsonPropertyName("is_bookmarked")]
        public bool IsBookmarked { get; set; }

        [JsonPropertyName("total_bookmarks")]
        public long TotalBookmarks { get; set; }

        [JsonPropertyName("total_view")]
        public long TotalView { get; set; }

        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        [JsonPropertyName("total_comments")]
        public long TotalComments { get; set; }

        [JsonPropertyName("is_muted")]
        public bool IsMuted { get; set; }

        [JsonPropertyName("is_mypixiv_only")]
        public bool IsMypixivOnly { get; set; }

        [JsonPropertyName("is_x_restricted")]
        public bool IsXRestricted { get; set; }

        public class ImageUrls
        {
            [JsonPropertyName("square_medium")]
            public string? SquareMedium { get; set; }

            [JsonPropertyName("medium")]
            public string? Medium { get; set; }

            [JsonPropertyName("large")]
            public string? Large { get; set; }
        }

        public class Series
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("title")]
            public string? Title { get; set; }
        }

        public class Tag
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("translated_name")]
            public string? TranslatedName { get; set; }

            [JsonPropertyName("added_by_uploaded_user")]
            public bool AddedByUploadedUser { get; set; }
        }

        public class UserInfo
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("account")]
            public string? Account { get; set; }

            [JsonPropertyName("profile_image_urls")]
            public ProfileImageUrls? ProfileImageUrls { get; set; }

            [JsonPropertyName("is_followed")]
            public bool IsFollowed { get; set; }
        }
    }
}