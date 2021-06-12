using System;
using System.Globalization;
using Autofac;
using JetBrains.Annotations;

namespace Mako
{
    [PublicAPI]
    public class MakoClient
    {
        public MakoClient(Session session, CultureInfo clientCulture)
        {
            Session = session;
            MakoServices = BuildContainer();
            ClientCulture = clientCulture;
        }

        private static IContainer BuildContainer()
        {
            // TODO
            throw new NotImplementedException();
        }
        
        public Session Session { get; private set; }
        
        internal IContainer MakoServices { get; init; }

        public CultureInfo ClientCulture { get; set; }

        internal TResult Resolve<TResult>() where TResult : notnull
        {
            return MakoServices.Resolve<TResult>();
        }
        
        internal TResult Resolve<TResult>(Type type) where TResult : notnull
        {
            return (TResult) MakoServices.Resolve(type);
        }
    }
}