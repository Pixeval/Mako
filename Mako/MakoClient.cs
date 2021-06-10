using System;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Mako
{
    [PublicAPI]
    public class MakoClient
    {
        public Session? Session { get; private set; }
        
        internal IServiceCollection MakoServices { get; init; }

        internal IServiceProvider MakoServiceProvider => MakoServices.BuildServiceProvider();
        
        public CultureInfo ClientCulture { get; set; }

        internal T? GetService<T>()
        {
            return GetService<T>(typeof(T));
        }

        internal T? GetService<T>(Type type)
        {
            return (T?) MakoServiceProvider.GetService(type);
        }
    }
}