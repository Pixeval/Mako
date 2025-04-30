// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

using System.Text.Json.Serialization;

namespace Mako.Net.Request;

public class ReverseSearchRequest(string apiKey)
{
    [JsonPropertyName("api_key")]
    public string ApiKey { get; } = apiKey;

    [JsonPropertyName("db")]
    public int Db { get; } = 999;

    [JsonPropertyName("output_type")]
    public OutputTypeEnum OutputType { get; } = OutputTypeEnum.Json;

    public enum OutputTypeEnum
    {
        Html, Xml, Json
    }
}
