// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Mako.Utilities;

public static class Jsons
{
    extension(JsonElement? jsonElement)
    {
        public IEnumerable<JsonProperty> EnumerateObjectOrEmpty() => jsonElement?.EnumerateObject() as IEnumerable<JsonProperty> ?? [];
    }

    extension(JsonElement jsonElement)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? GetPropertyString() => jsonElement.GetString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? GetPropertyString(string prop) => jsonElement.GetProperty(prop).GetString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonElement? GetPropertyOrNull(string prop) => jsonElement.TryGetProperty(prop, out var result) ? result : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetPropertyLong(string prop) => jsonElement.GetProperty(prop).GetInt64();
    }

    extension(JsonProperty jsonProperty)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonElement GetProperty(string prop) => jsonProperty.Value.GetProperty(prop);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonElement? GetPropertyOrNull(string prop) => jsonProperty.Value.TryGetProperty(prop, out var result) ? result : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetPropertyString() => jsonProperty.Value.ToString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetPropertyString(string prop) => jsonProperty.Value.GetProperty(prop).ToString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetPropertyLong(string prop) => jsonProperty.Value.GetProperty(prop).GetInt64();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTimeOffset GetPropertyDateTimeOffset() => jsonProperty.Value.GetDateTimeOffset();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTimeOffset GetPropertyDateTimeOffset(string prop) => jsonProperty.Value.GetProperty(prop).GetDateTimeOffset();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime GetPropertyDateTime() => jsonProperty.Value.GetDateTime();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime GetPropertyDateTime(string prop) => jsonProperty.Value.GetProperty(prop).GetDateTime();
    }
}
