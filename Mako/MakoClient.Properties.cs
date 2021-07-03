using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading;
using Autofac;
using Mako.Engines;
using Mako.Preference;

namespace Mako
{
    public partial class MakoClient
    {
        private readonly List<IEngineHandleSource> _runningInstances = new();

        /// <summary>
        ///     The globally unique ID of current <see cref="MakoClient" />
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        public Session Session { get; private set; }

        public MakoClientConfiguration Configuration { get; set; }

        internal ISessionUpdate SessionUpdater { get; }

        /// <summary>
        ///     The IoC container
        /// </summary>
        internal IContainer MakoServices { get; init; }

        internal static MemoryCache MemoryCache { get; }

        /// <summary>
        ///     The <see cref="CancellationTokenSource" /> that is used to cancel ths <see cref="MakoClient" />\
        ///     and all of its running engines
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}