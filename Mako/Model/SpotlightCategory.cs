// Copyright (c) Mako.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Mako.Model;

public enum SpotlightCategory
{
    [Description("all")]
    All,

    [Description("spotlight")]
    Spotlight,

    [Description("tutorial")]
    Tutorial,

    [Description("inspiration")]
    Inspiration
}
