// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Mako.Net.Request;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
public record RemoveFollowUserRequest([property: JsonPropertyName("user_id")] long UserId);
