using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using Autofac;
using JetBrains.Annotations;
using Mako.Engines;
using Mako.Engines.Implements;
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
            MemoryCache = new MemoryCache(Id.ToString(), new NameValueCollection {["cacheMemoryLimitMegabytes"] = "100"});
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

            builder.Register(static c => RestService.For<IAppApiProtocol>(c.ResolveKeyed<HttpClient>(MakoApiKind.AppApi)));
            return builder.Build();
        }

        public Guid Id { get; } = Guid.NewGuid();

        public Session Session { get; private set; }

        public CultureInfo ClientCulture { get; set; }

        /// <summary>
        /// 正在执行的所有实例
        /// </summary>
        private readonly List<IEngineHandleSource> _runningInstances = new();

        public bool IsCanceled { get; set; }
        
        internal IContainer MakoServices { get; init; }
        
        internal MemoryCache MemoryCache { get; }

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

        internal void Cache<T>(CacheType type, string key, T item) where T : notnull
        {
            MemoryCache.AddWithRegionName(key, item, new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(5)
            }, type.ToString());
        }
        
        internal T? GetCached<T>(CacheType type, string key) where T : notnull
        {
            return (T?) MemoryCache.GetWithRegionName(key, type.ToString());
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

        public IFetchEngine<Illustration> Bookmarks(string uid, PrivacyPolicy privacyPolicy)
        {
            return (IFetchEngine<Illustration>?) GetCached<AdaptedComputedFetchEngine<Illustration>>(CacheType.Bookmarks, Caches.CreateBookmarkCacheKey(uid))
                   ?? new BookmarkEngine(this, uid, privacyPolicy, new EngineHandle(handle =>
                   {
                       CancelInstance(handle);
                       TryCache(CacheType.Bookmarks, handle.Cache.Cast<Illustration>(), Caches.CreateBookmarkCacheKey(uid));
                   })).Apply(RegisterInstance);
        }

        public IFetchEngine<Illustration> Search(
            string tag,
            int start = 0,
            int pages = 100,
            SearchTagMatchOption matchOption = SearchTagMatchOption.TitleAndCaption,
            IllustrationSortOption? sortOption = null,
            SearchDuration? searchDuration = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Search function is inadequate for caching
            return new SearchEngine(this, new EngineHandle(CancelInstance), matchOption, tag, start, pages, sortOption, searchDuration, startDate, endDate);
        }

        public IFetchEngine<Illustration> Ranking(RankOption rankOption, DateTime dateTime)
        {
            if (DateTime.Today - dateTime.Date > TimeSpan.FromDays(2))
            {
                throw new RankingDateOutOfRangeException();
            }

            var key = Caches.CreateRankingCacheKey(rankOption, dateTime);
            return (IFetchEngine<Illustration>?) GetCached<AdaptedComputedFetchEngine<Illustration>>(CacheType.Ranking, key)
                   ?? new RankingEngine(this, rankOption, dateTime, new EngineHandle(handle =>
                   {
                       CancelInstance(handle);
                       TryCache(CacheType.Ranking, handle.Cache.Cast<Illustration>(), key);
                   }));
        }

        public IFetchEngine<Illustration> Recommends()
        {
            return new RecommendsEngine(this, new EngineHandle(CancelInstance));
        }

        public IFetchEngine<User> RecommendIllustrators()
        {
            return new RecommendIllustratorEngine(this, new EngineHandle(CancelInstance));
        }

        public IFetchEngine<SpotlightArticle> Spotlights()
        {
            return new SpotlightArticleEngine(this, new EngineHandle(CancelInstance));
        }

        public IFetchEngine<Feed> Feeds()
        {
            return new UserFeedsEngine(this, new EngineHandle(CancelInstance));
        }

        public IFetchEngine<T>? GetByHandle<T>(EngineHandle handle)
        {
            return _runningInstances.FirstOrDefault(h => h.EngineHandle == handle) as IFetchEngine<T>;
        }
    }
}