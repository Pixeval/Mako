// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record PixivRelatedUsersResponse
{
    [JsonPropertyName("user_previews")]
    public required IReadOnlyList<User> Users { get; set; } = [];
}
