// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

namespace Mako.Utilities;

internal interface IDefaultFactory<out TSelf> where TSelf : IDefaultFactory<TSelf>
{
    static abstract TSelf CreateDefault();
}
