﻿// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

namespace Mako.Engine;

public interface INotifyCompletion
{
    bool IsCompleted { get; set; }
}