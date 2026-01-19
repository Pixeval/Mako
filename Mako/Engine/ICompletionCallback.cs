// Copyright (c) Mako.
// Licensed under the MIT License.

namespace Mako.Engine;

public interface ICompletionCallback<in T>
{
    void OnCompletion(T param);
}