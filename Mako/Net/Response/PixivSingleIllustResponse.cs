// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Mako.Model;
using Mako.Utilities;

namespace Mako.Net.Response;

[Factory]
public partial record PixivSingleIllustResponse
{
    [JsonPropertyName("illust")]
    public required Illustration Illust { get; set; }
}
