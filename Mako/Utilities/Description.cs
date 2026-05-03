// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Mako.Utilities;

internal static class DescriptionHelper
{
    extension<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(TEnum @enum) where TEnum : Enum
    {
        public string GetDescription()
        {
            return @enum.TryGetDescription() ?? throw new InvalidOperationException("Attribute not found");
        }

        public string? TryGetDescription()
        {
            return (typeof(TEnum).GetField(@enum.ToString())?.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description;
        }
    }
}
