using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines
{
    /// <summary>
    /// 一个可以不停的搜索新页面直到不再有更多页面可以被抓取的迭代器，一个页面可以包含多个搜索结果
    /// </summary>
    /// <typeparam name="TEntity">搜索结果对应的实体类</typeparam>
    /// <typeparam name="TRawEntity">页面对应的实体类</typeparam>
    /// <typeparam name="TFetchEngine">搜索引擎</typeparam>
    internal abstract class RecursivePixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine> : AbstractPixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine>
        where TEntity : class?
        where TFetchEngine : class, IFetchEngine<TEntity>
    {
        protected TRawEntity? Entity { get; private set; }

        protected RecursivePixivAsyncEnumerator(TFetchEngine pixivFetchEngine, MakoApiKind makoApiKind)
            : base(pixivFetchEngine, makoApiKind)
        {
        }

        /// <summary>
        /// 获取下一个页面的URL
        /// </summary>
        /// <returns>下一个页面的URL</returns>
        protected abstract string? NextUrl(TRawEntity? rawEntity);

        /// <summary>
        /// 获取第一个页面的URL
        /// </summary>
        /// <returns>第一个页面的URL</returns>
        protected abstract string InitialUrl();

        /// <summary>
        /// 从新页面的返回结果中获取该页面的所有搜索结果的迭代器
        /// </summary>
        /// <returns>所有搜索结果的迭代器</returns>
        protected abstract IEnumerator<TEntity>? GetNewEnumerator(TRawEntity? rawEntity);

        /// <summary>
        /// 指示是否还有下一页
        /// </summary>
        /// <returns>是否还有下一页</returns>
        protected virtual bool HasNextPage() => NextUrl(Entity).IsNotNullOrEmpty();

        /// <summary>
        /// 指示是否还有下一个结果，该函数在搜索结果数被限制的时候很有用
        /// </summary>
        /// <returns>是否还有下一个结果</returns>
        protected virtual bool HasNext() => true;

        /// <summary>
        /// 获取下一个搜索结果，如果已经到达了当前页的末尾，则请求一个新的页面并返回新页面的第一个搜索结果
        /// </summary>
        /// <remarks>
        /// 如果该函数发现搜索引擎已经搜索到了末尾，也即无法再提供更多的结果，则将会设置<see cref="EngineHandle.IsCompleted"/>属性
        /// 以标明该搜索引擎已经结束运行
        /// </remarks>
        /// <returns>是否还有更多的结果</returns>
        public override async ValueTask<bool> MoveNextAsync()
        {
            if (IsCancellationRequested || !HasNext())
            {
                PixivFetchEngine.EngineHandle.Complete(); // Set the state of the 'PixivFetchEngine' to Completed
                return false;
            }

            if (Entity is null)
            {
                var first = InitialUrl();
                switch (await GetJsonResponse(first))
                {
                    case Result<TRawEntity>.Success (var raw):
                        Update(raw);
                        break;
                    case Result<TRawEntity>.Failure (var exception):
                        if (exception is { } e)
                        {
                            throw e;
                        }
                        PixivFetchEngine.EngineHandle.Complete();
                        return false;
                }
            }

            if (CurrentEntityEnumerator!.MoveNext()) // If the enumerator can proceeds then return true
            {
                TryCacheCurrent(); // Cache if allowed in session
                return true;
            }

            if (!HasNextPage() || !HasNext()) // Check if there are more pages, return false if not
            {
                PixivFetchEngine.EngineHandle.Complete();
                return false;
            }

            if (await GetJsonResponse(NextUrl(Entity)!) is Result<TRawEntity>.Success (var value)) // Else request a new page
            {
                Update(value);
                TryCacheCurrent();
                return true;
            }

            PixivFetchEngine.EngineHandle.Complete();
            return false;
        }

        private void TryCacheCurrent()
        {
            if (PixivFetchEngine.MakoClient.Session.AllowCache)
            {
                PixivFetchEngine.EngineHandle.CacheValue(Current);
            }
        }

        /// <summary>
        /// 每申请一个新的页面后负责更新迭代器，实体对象和页数
        /// </summary>
        /// <param name="rawEntity">新页面的请求结果</param>
        protected override void Update(TRawEntity rawEntity)
        {
            Entity = rawEntity;
            CurrentEntityEnumerator = GetNewEnumerator(rawEntity) ?? EmptyEnumerators<TEntity>.Sync;
            PixivFetchEngine!.RequestedPages++;
        }
    }
}