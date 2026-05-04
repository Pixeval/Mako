// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Net.Request;
using Mako.Net.Response;

namespace Mako;

[JsonSerializable(typeof(AutoCompletionResponse))]
[JsonSerializable(typeof(BookmarkTagResponse))]
[JsonSerializable(typeof(CommentResponse))]
[JsonSerializable(typeof(IllustrationResponse))]
[JsonSerializable(typeof(NovelResponse))]
[JsonSerializable(typeof(RelatedUsersResponse))]
[JsonSerializable(typeof(SingleIllustrationResponse))]
[JsonSerializable(typeof(SingleNovelResponse))]
[JsonSerializable(typeof(SingleUserResponse))]
[JsonSerializable(typeof(PixivisionDetailResponse))]
[JsonSerializable(typeof(SpotlightResponse))]
[JsonSerializable(typeof(UserResponse))]
[JsonSerializable(typeof(PostCommentResponse))]
[JsonSerializable(typeof(ReverseSearchResponse))]
[JsonSerializable(typeof(TrendingTagResponse))]
[JsonSerializable(typeof(UgoiraMetadataResponse))]
[JsonSerializable(typeof(UserSpecifiedBookmarkTagResponse))]
[JsonSerializable(typeof(WebApiBookmarksWithTagResponse))]
[JsonSerializable(typeof(ShowAiSettingsResponse))]
[JsonSerializable(typeof(RestrictedModeSettingsResponse))]
[JsonSerializable(typeof(SeriesResponse))]
[JsonSerializable(typeof(MangaSeriesDetailResponse))]
[JsonSerializable(typeof(NovelSeriesDetailResponse))]

[JsonSerializable(typeof(Feed))]
[JsonSerializable(typeof(NovelContent))]
[JsonSerializable(typeof(SearchOptions))]

[JsonSerializable(typeof(AddIllustrationBookmarkRequest))]
[JsonSerializable(typeof(AddNormalIllustrationCommentRequest))]
[JsonSerializable(typeof(AddNormalNovelCommentRequest))]
[JsonSerializable(typeof(AddNovelBookmarkRequest))]
[JsonSerializable(typeof(AddStampIllustrationCommentRequest))]
[JsonSerializable(typeof(AddStampNovelCommentRequest))]
[JsonSerializable(typeof(FollowUserRequest))]
[JsonSerializable(typeof(ReverseSearchRequest))]

[JsonSerializable(typeof(RefreshSessionRequest))]
[JsonSerializable(typeof(RequestSessionRequest))]
[JsonSerializable(typeof(TokenResponse))]
internal partial class MakoJsonSerializerContext : JsonSerializerContext;

public class SnakeCaseLowerEnumConverter<T>() : JsonStringEnumConverter<T>(JsonNamingPolicy.SnakeCaseLower) where T : struct, Enum;
