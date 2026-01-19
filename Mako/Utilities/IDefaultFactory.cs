// Copyright (c) Mako.
// Licensed under the MIT License.

namespace Mako.Utilities;

internal interface IDefaultFactory<out TSelf> where TSelf : IDefaultFactory<TSelf>
{
    static abstract TSelf CreateDefault();
}
