// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

namespace Mako.Net.Response;

internal interface ISingleResultResponse<out T>
{
    T Content { get; }
}
