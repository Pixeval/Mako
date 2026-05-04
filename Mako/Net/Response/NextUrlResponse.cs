// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mako.Utilities;

namespace Mako.Net.Response;

internal interface IPixivNextUrlResponse<TEntity> where TEntity : class
{
    [JsonPropertyName("next_url")]
    string? NextUrl { get; set; }

    /// <summary>
    /// 设置默认值为 [] 以便 <see cref="FactoryAttribute"/>可以生成正确的默认值，以避免null警告
    /// </summary>
    IReadOnlyList<TEntity> Entities { get; set; }
}
