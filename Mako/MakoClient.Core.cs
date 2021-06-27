using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using Autofac;
using JetBrains.Annotations;
using Mako.Engines;
using Mako.Engines.Implements;
using Mako.Net;
using Mako.Net.Protocol;
using Mako.Util;
using Refit;

namespace Mako
{
    [PublicAPI]
    public partial class MakoClient : ICancellable
    {
        static MakoClient()
        {
            CacheLimitsInMegabytes = 50;
            MemoryCache = new MemoryCache("MakoCache", new NameValueCollection {["cacheMemoryLimitMegabytes"] = CacheLimitsInMegabytes.ToString()});
        }
        
        public MakoClient(Session session, CultureInfo clientCulture)
        {
            Session = session;
            MakoServices = BuildContainer();
            ClientCulture = clientCulture;
            CancellationTokenSource = new CancellationTokenSource();
            // each running instance has its own 'CancellationTokenSource', because we want to have the ability to cancel a particular instance
            // while also be able to cancel all of them from 'MakoClient'
            CancellationTokenSource.Token.Register(() => _runningInstances.ForEach(instance => instance.EngineHandle.Cancel()));
        }

        /// <summary>
        /// 注入必要的依赖
        /// </summary>
        /// <returns></returns>
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
                .PropertiesAutowired(static(info, _) => info.PropertyType == typeof(MakoClient))
                .SingleInstance();
            builder.Register(static c => new RetryHttpClientHandler(c.Resolve<PixivImageHttpMessageHandler>()))
                .Keyed<HttpMessageHandler>(typeof(PixivImageHttpMessageHandler))
                .As<HttpMessageHandler>()
                .PropertiesAutowired(static(info, _) => info.PropertyType == typeof(MakoClient))
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

            builder.Register(static c =>
            {
                var context = c.Resolve<IComponentContext>(); // or a System.ObjectDisposedException will thrown because the 'c' cannot be hold
                return RestService.For<IAppApiProtocol>(c.ResolveKeyed<HttpClient>(MakoApiKind.AppApi), new RefitSettings
                {
                    ExceptionFactory = async message => !message.IsSuccessStatusCode ? await MakoNetworkException.FromHttpResponseMessage(message, context.Resolve<MakoClient>().Session.Bypass) : null
                });
            });
            return builder.Build();
        }

        public void Cancel()
        {
            CancellationTokenSource.Cancel();
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

        private string CreateCacheRegionForCurrent(string secondary)
        {
            return $"{Id}::{secondary}";
        }
        
        internal void Cache<T>(CacheType type, string key, T item) where T : notnull
        {
            
            MemoryCache.AddWithRegionName(key, item, new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(5)
            }, CreateCacheRegionForCurrent(type.ToString()));
        }
        
        internal T? GetCached<T>(CacheType type, string key) where T : notnull
        {
            return (T?) MemoryCache.GetWithRegionName(key, CreateCacheRegionForCurrent(type.ToString()));
        }

        private void RegisterInstance(IEngineHandleSource engineHandleSource) => _runningInstances.Add(engineHandleSource);

        private void CancelInstance(EngineHandle handle) => _runningInstances.RemoveAll(instance => instance.EngineHandle == handle);

        private void TryCache<T>(CacheType type, IEnumerable<T> enumerable, string key)
        {
            if (Session.AllowCache)
            {
                Cache(type, key, new AdaptedComputedFetchEngine<T>(enumerable));
            }
        }

        // PrivacyPolicy.Private is only allowed when the uid is pointing to yourself
        private bool CheckPrivacyPolicy(string uid, PrivacyPolicy privacyPolicy)
        {
            return !(privacyPolicy == PrivacyPolicy.Private && Session.Id! != uid);
        }

        public IFetchEngine<T>? GetByHandle<T>(EngineHandle handle)
        {
            return _runningInstances.FirstOrDefault(h => h.EngineHandle == handle) as IFetchEngine<T>;
        }
    }
}