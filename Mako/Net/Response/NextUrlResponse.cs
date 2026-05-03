// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mako.Net.Response;

public interface IPixivNextUrlResponse<TEntity> where TEntity : class
{
    [JsonPropertyName("next_url")]
    string? NextUrl { get; set; }

    IReadOnlyList<TEntity> Entities { get; set; }
}
