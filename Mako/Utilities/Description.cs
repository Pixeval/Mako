// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Mako.Utilities;

internal static class DescriptionHelper
{
    extension<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(TEnum @enum) where TEnum : struct, Enum
    {
        public string GetEnumMemberName()
        {
            return @enum.TryGetEnumMemberName() ?? throw new InvalidOperationException("Attribute not found");
        }

        public string? TryGetEnumMemberName()
        {
            return (typeof(TEnum).GetField(@enum.ToString())?.GetCustomAttribute(typeof(JsonStringEnumMemberNameAttribute)) as JsonStringEnumMemberNameAttribute)?.Name;
        }
    }
}
