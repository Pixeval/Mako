using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Autofac;
using JetBrains.Annotations;
using Mako.Engines;
using Mako.Model;
using Mako.Net;
using Mako.Net.Protocol;
using Mako.Util;
using Refit;

namespace Mako
{
    [PublicAPI]
    public class MakoClient : ICancellable
    {
        public MakoClient(Session session, CultureInfo clientCulture)
        {
            Session = session;
            MakoServices = BuildContainer();
            ClientCulture = clientCulture;
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(this).SingleInstance();

            builder.RegisterType<PixivApiNameResolver>().SingleInstance();
            builder.RegisterType<PixivImageNameResolver>().SingleInstance();
            builder.RegisterType<LocalMachineNameResolver>().SingleInstance();

            builder.RegisterType<IllustrationPopularityComparator>().SingleInstance();
            builder.RegisterType<IllustrationPublishDateComparator>().SingleInstance();

            builder.RegisterType<PixivApiHttpMessageHandler>().SingleInstance();
            builder.RegisterType<PixivImageHttpMessageHandler>().SingleInstance();

            builder.Register(static c => new RetryHttpClientHandler(c.Resolve<PixivApiHttpMessageHandler>()))
                .Keyed<HttpMessageHandler>(typeof(PixivApiHttpMessageHandler))
                .As<HttpMessageHandler>()
                .PropertiesAutowired(static (info, _) => info.PropertyType == typeof(MakoClient))
                .SingleInstance();
            builder.Register(static c => new RetryHttpClientHandler(c.Resolve<PixivImageHttpMessageHandler>()))
                .Keyed<HttpMessageHandler>(typeof(PixivImageHttpMessageHandler))
                .As<HttpMessageHandler>()
                .PropertiesAutowired(static (info, _) => info.PropertyType == typeof(MakoClient))
                .SingleInstance();
            builder.Register(static c => MakoHttpClient.Create(c.ResolveKeyed<HttpMessageHandler>(typeof(PixivApiHttpMessageHandler)),
                    static client => client.BaseAddress = new Uri(MakoHttpOptions.AppApiBaseUrl)))
                .Keyed<HttpClient>(MakoApiKind.AppApi)
                .As<HttpClient>()
                .SingleInstance();
            builder.Register(static c => MakoHttpClient.Create(c.ResolveKeyed<HttpMessageHandler>(typeof(PixivApiHttpMessageHandler)),
                    static client => client.BaseAddress = new Uri(MakoHttpOptions.WebApiBaseUrl)))
                .Keyed<HttpClient>(MakoApiKind.WebApi)
                .As<HttpClient>()
                .SingleInstance();
            builder.Register(static c => MakoHttpClient.Create(c.ResolveKeyed<HttpMessageHandler>(typeof(PixivApiHttpMessageHandler)),
                    static client =>
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://www.pixiv.net");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "PixivIOSApp/5.8.7");
                    }))
                .Keyed<HttpClient>(MakoApiKind.ImageApi)
                .As<HttpClient>()
                .SingleInstance();

            builder.Register(static c => RestService.For<IAppApiProtocol>(c.ResolveKeyed<HttpClient>(MakoApiKind.AppApi)));
            return builder.Build();
        }

        public Guid Id { get; } = Guid.NewGuid();

        public Session Session { get; private set; }
        
        internal IContainer MakoServices { get; init; }

        public CultureInfo ClientCulture { get; set; }

        /// <summary>
        /// 正在执行的所有实例
        /// </summary>
        private readonly List<IEngineHandleSource> _runningInstances = new();

        public bool IsCanceled { get; set; }

        public void Cancel()
        {
            IsCanceled = true;
            _runningInstances.ForEach(instance => instance.EngineHandle.Cancel());
        }
        
        internal TResult Resolve<TResult>() where TResult : notnull
        {
            return MakoServices.Resolve<TResult>();
        }
        
        internal TResult ResolveKeyed<TResult>(object key) where TResult : notnull
        {
            return MakoServices.ResolveKeyed<TResult>(key);
        }
        
        internal TResult Resolve<TResult>(Type type) where TResult : notnull
        {
            return (TResult) MakoServices.Resolve(type);
        }

        private void RegisterInstance(IEngineHandleSource engineHandleSource) => _runningInstances.Add(engineHandleSource);

        private void CancelInstance(EngineHandle handle) => _runningInstances.RemoveAll(instance => instance.EngineHandle == handle);
        
        public IFetchEngine<Illustration> Bookmarks(string uid, PrivacyPolicy privacyPolicy)
        {
            return new BookmarkEngine(this, uid, privacyPolicy, new EngineHandle(CancelInstance)).Apply(RegisterInstance);
        }

        public IFetchEngine<T>? GetByHandle<T>(EngineHandle handle)
        {
            return _runningInstances.FirstOrDefault(h => h.EngineHandle == handle) as IFetchEngine<T>;
        }
    }
}