#region Copyright (c) Pixeval/Mako

// MIT License
// 
// Copyright (c) Pixeval 2021 Mako/MakoClient.Core.cs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Mako.Engine;
using Mako.Engine.Implements;
using Mako.Net;
using Mako.Net.EndPoints;
using Mako.Preference;
using Mako.Util;
using Refit;

namespace Mako
{
    [PublicAPI]
    public partial class MakoClient : ICancellable
    {
        /// <summary>
        ///     Creates the cache of current <see cref="MakoClient" />
        /// </summary>
        static MakoClient()
        {
            MemoryCache = new MemoryCache("MakoCache", new NameValueCollection {["cacheMemoryLimitMegabytes"] = "50"});
        }

        /// <summary>
        ///     Create an new <see cref="MakoClient" /> based on given <see cref="Configuration" />, <see cref="Session" />, and
        ///     <see cref="ISessionUpdate" />
        /// </summary>
        /// <remarks>
        ///     The <see cref="MakoClient" /> is not responsible for the <see cref="Session" />'s refreshment, you need to check
        ///     the
        ///     <see cref="P:Session.Expire" /> and call <see cref="RefreshSession(Mako.Preference.Session)" /> or
        ///     <see cref="RefreshSessionAsync" />
        ///     periodically
        /// </remarks>
        /// <param name="session">The <see cref="Mako.Preference.Session" /></param>
        /// <param name="configuration">The <see cref="Configuration" /></param>
        /// <param name="sessionUpdater">The updater of <see cref="Mako.Preference.Session" /></param>
        public MakoClient(Session session, MakoClientConfiguration configuration, ISessionUpdate? sessionUpdater = null)
        {
            SessionUpdater = sessionUpdater ?? new RefreshTokenSessionUpdate();
            Session = session;
            MakoServices = BuildContainer();
            Configuration = configuration;
            CancellationTokenSource = new CancellationTokenSource();
            // each running instance has its own 'CancellationTokenSource', because we want to have the ability to cancel a particular instance
            // while also be able to cancel all of them from 'MakoClient'
            CancellationTokenSource.Token.Register(() => _runningInstances.ForEach(instance => instance.EngineHandle.Cancel()));
        }

        /// <summary>
        ///     Creates a <see cref="MakoClient" /> based on given <see cref="Session" /> and <see cref="ISessionUpdate" />, the
        ///     configurations will stay
        ///     as default
        /// </summary>
        /// <remarks>
        ///     The <see cref="MakoClient" /> is not responsible for the <see cref="Session" />'s refreshment, you need to check
        ///     the
        ///     <see cref="P:Session.Expire" /> and call <see cref="RefreshSession(Mako.Preference.Session)" /> or
        ///     <see cref="RefreshSessionAsync" />
        ///     periodically
        /// </remarks>
        /// <param name="session">The <see cref="Mako.Preference.Session" /></param>
        /// <param name="sessionUpdater">The updater of <see cref="Mako.Preference.Session" /></param>
        public MakoClient(Session session, ISessionUpdate? sessionUpdater = null)
        {
            SessionUpdater = sessionUpdater ?? new RefreshTokenSessionUpdate();
            Session = session;
            MakoServices = BuildContainer();
            Configuration = new MakoClientConfiguration();
            CancellationTokenSource = new CancellationTokenSource();
            // each running instance has its own 'CancellationTokenSource', because we want to have the ability to cancel a particular instance
            // while also be able to cancel all of them from 'MakoClient'
            CancellationTokenSource.Token.Register(() => _runningInstances.ForEach(instance => instance.EngineHandle.Cancel()));
        }

        /// <summary>
        ///     Injects necessary dependencies
        /// </summary>
        /// <returns>The <see cref="IContainer" /> contains all the required dependencies</returns>
        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(this).SingleInstance();

            builder.RegisterType<PixivApiNameResolver>().SingleInstance();
            builder.RegisterType<PixivImageNameResolver>().SingleInstance();
            builder.RegisterType<LocalMachineNameResolver>().SingleInstance();

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
                return RestService.For<IAppApiEndPoint>(c.ResolveKeyed<HttpClient>(MakoApiKind.AppApi), new RefitSettings
                {
                    ExceptionFactory = async message => !message.IsSuccessStatusCode ? await MakoNetworkException.FromHttpResponseMessageAsync(message, context.Resolve<MakoClient>().Configuration.Bypass).ConfigureAwait(false) : null
                });
            });

            builder.Register(static c =>
            {
                var context = c.Resolve<IComponentContext>(); // or a System.ObjectDisposedException will thrown because the 'c' cannot be hold
                return RestService.For<IAuthEndPoint>(c.ResolveKeyed<HttpClient>(MakoApiKind.AppApi), new RefitSettings
                {
                    ExceptionFactory = async message => !message.IsSuccessStatusCode ? await MakoNetworkException.FromHttpResponseMessageAsync(message, context.Resolve<MakoClient>().Configuration.Bypass).ConfigureAwait(false) : null
                });
            });
            return builder.Build();
        }

        /// <summary>
        ///     Cancels this <see cref="MakoClient" />, including all of the running instances, the
        ///     <see cref="Session" /> will be reset to its default value, the <see cref="MakoClient" />
        ///     will unable to be used again after calling this method
        /// </summary>
        public void Cancel()
        {
            Session = new Session();
            CancellationTokenSource.Cancel();
        }

        // Ensures the current instances hasn't been cancelled
        private void EnsureNotCancelled()
        {
            if (CancellationTokenSource.IsCancellationRequested)
            {
                throw new OperationCanceledException($"MakoClient({Id}) has been cancelled");
            }
        }

        // Resolves a dependency of type 'TResult'
        internal TResult Resolve<TResult>() where TResult : notnull
        {
            return MakoServices.Resolve<TResult>();
        }

        // Resolves a key-bounded dependency of type 'TResult'
        internal TResult ResolveKeyed<TResult>(object key) where TResult : notnull
        {
            return MakoServices.ResolveKeyed<TResult>(key);
        }

        // Resolves a dependency of type 'type'
        internal TResult Resolve<TResult>(Type type) where TResult : notnull
        {
            return (TResult) MakoServices.Resolve(type);
        }

        // Creates the namespace (region) of the current 'MakoClient'
        private string CreateCacheRegionForCurrent(string secondary)
        {
            return $"{Id}::{secondary}";
        }

        // Caches an 'item'
        internal void Cache<T>(CacheType type, string key, T item) where T : notnull
        {
            MemoryCache.AddWithRegionName(key, item, new CacheItemPolicy
            {
                SlidingExpiration = Configuration.CacheEntrySlidingExpiration
            }, CreateCacheRegionForCurrent(type.ToString()));
        }

        // Gets an cached item or null
        internal T? GetCached<T>(CacheType type, string key) where T : notnull
        {
            return (T?) MemoryCache.GetWithRegionName(key, CreateCacheRegionForCurrent(type.ToString()));
        }

        // registers an instance to the running instances list
        private void RegisterInstance(IEngineHandleSource engineHandleSource)
        {
            _runningInstances.Add(engineHandleSource);
        }

        // removes an instance from the running instances list
        private void CancelInstance(EngineHandle handle)
        {
            _runningInstances.RemoveAll(instance => instance.EngineHandle == handle);
        }

        // Cache the results of 'IFetchEngine<out E>' if and only if 'Configuration.AllowCache' is set
        private void TryCache<T>(CacheType type, IEnumerable<T> enumerable, string key)
        {
            if (Configuration.AllowCache)
            {
                Cache(type, key, new AdaptedComputedFetchEngine<T>(enumerable));
            }
        }

        // PrivacyPolicy.Private is only allowed when the uid is pointing to yourself
        private bool CheckPrivacyPolicy(string uid, PrivacyPolicy privacyPolicy)
        {
            return !(privacyPolicy == PrivacyPolicy.Private && Session.Id! != uid);
        }

        /// <summary>
        ///     Gets a registered <see cref="IFetchEngine{E}" /> by its <see cref="EngineHandle" />
        /// </summary>
        /// <param name="handle">The <see cref="EngineHandle" /> of the <see cref="IFetchEngine{E}" /></param>
        /// <typeparam name="T">The type of the results of the <see cref="IFetchEngine{E}" /></typeparam>
        /// <returns>The <see cref="IFetchEngine{E}" /> instance</returns>
        public IFetchEngine<T>? GetByHandle<T>(EngineHandle handle)
        {
            return _runningInstances.FirstOrDefault(h => h.EngineHandle == handle) as IFetchEngine<T>;
        }

        /// <summary>
        ///     Acquires a configured <see cref="HttpClient" /> for the network traffics
        /// </summary>
        /// <param name="makoApiKind">The kind of API that is going to be used by the request</param>
        /// <returns>The <see cref="HttpClient" /> corresponding to <paramref name="makoApiKind" /></returns>
        public HttpClient GetMakoHttpClient(MakoApiKind makoApiKind)
        {
            return ResolveKeyed<HttpClient>(makoApiKind);
        }

        /// <summary>
        ///     Sets the <see cref="Session" /> to a new value
        /// </summary>
        /// <param name="newSession">The new <see cref="Mako.Preference.Session" /></param>
        public void RefreshSession(Session newSession)
        {
            Session = newSession;
        }

        /// <summary>
        ///     Refresh session using the provided <see cref="ISessionUpdate" />
        /// </summary>
        public async Task RefreshSessionAsync()
        {
            Session = await SessionUpdater.RefreshAsync(this).ConfigureAwait(false);
        }
    }
}