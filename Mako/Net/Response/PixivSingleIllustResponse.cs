// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

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
