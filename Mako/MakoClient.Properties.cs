// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;
using Mako.Engine;
using Mako.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Misaki;

namespace Mako;

public partial class MakoClient
{
    private readonly List<IEngineHandleSource> _runningInstances = [];

    public TokenUser? Me { get; private set; }

    public MakoConfiguration Configuration { get; set; }

    public ILogger Logger { get; }

    /// <summary>
    /// The IoC container
    /// </summary>
    internal ServiceCollection Services { get; } = [];

    public ServiceProvider Provider { get; } = null!;

    public ClientStatus Status { get; private set; }

    string IPlatformInfo.Platform => IPlatformInfo.Pixiv;

    public enum ClientStatus
    {
        Created,
        Built,
        Disposed
    }
}
