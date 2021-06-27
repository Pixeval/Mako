using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Caching;
using System.Threading;
using Autofac;
using Mako.Engines;

namespace Mako
{
    public partial class MakoClient
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Session Session { get; private set; }

        public CultureInfo ClientCulture { get; set; }

        /// <summary>
        /// 正在执行的所有实例
        /// </summary>
        private readonly List<IEngineHandleSource> _runningInstances = new();

        public CancellationTokenSource CancellationTokenSource { get; set; }
        
        internal IContainer MakoServices { get; init; }
        
        internal static MemoryCache MemoryCache { get; }
        
        public static int CacheLimitsInMegabytes { get; set; }
    }
}