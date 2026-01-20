// Copyright (c) Mako.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Mako.Engine;
using Mako.Model;
using Mako.Net;
using Mako.Preference;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mako;

public partial class MakoClient
{
    private readonly List<IEngineHandleSource> _runningInstances = [];

    /// <summary>
    /// The globally unique ID of current <see cref="MakoClient" />
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    public TokenUser Me => Provider.GetRequiredService<PixivTokenProvider>().Me;

    public TokenUser? TryGetMe() => Provider.GetService<PixivTokenProvider>()?.Me;

    public MakoClientConfiguration Configuration { get; set; }

    public ILogger Logger { get; }

    /// <summary>
    /// The IoC container
    /// </summary>
    internal ServiceCollection Services { get; } = [];

    public ServiceProvider Provider { get; private set; } = null!;

    public bool IsBuilt { get; private set; }

    public bool IsCancelled { get; private set; }
}
