// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
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

    /// <summary>
    /// The globally unique ID of current <see cref="MakoClient" />
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    public TokenUser? Me
    {
        get;
        internal set
        {
            if (field == value)
                return;
            field = value;
            OnTokenRefreshed(value);
        }
    }

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
