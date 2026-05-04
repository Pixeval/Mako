// Copyright (c) Mako.
// Licensed under the MIT License.

using System;

namespace Mako.Utilities;

[AttributeUsage(AttributeTargets.Constructor)]
internal class MakoExtensionConstructorAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal class GeneratedMakoExtensionAttribute : Attribute;
