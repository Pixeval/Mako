﻿using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines
{
    /// <summary>
    /// 和<see cref="AbstractPixivFetchEngine{E}"/>协作的枚举器，抽象了一个搜索任务，该枚举器包含了搜索的核心逻辑
    /// <para>Pixiv的APP API通常使用如下的JSON格式来表示一页的搜索结果</para>
    /// <code>
    /// {
    ///     "contents": [单个对象的列表，这里的对象可以是画作，可以是用户],
    ///     "nextUrl": "&lt;下一页的URL&gt;"
    /// }
    /// </code>
    /// <see cref="AbstractPixivAsyncEnumerator{E,R}"/>提供了一个枚举器以枚举JSON资源的
    /// <c>contents</c>部分，该类包含四个主要任务：
    /// <list type="number">
    ///     <item>
    ///         <term>抓取：</term>
    ///         <description>从对应的API抓取JSON资源，并将JSON资源反序列化为对应的实体类对象(泛型参数<typeparamref name="TRawEntity"/>)</description>
    ///     </item>
    ///     <item>
    ///         <term>解析：</term>
    ///         <description>将第一步中得到的实体类对象，翻译为简化后的统一对象以方便操作(泛型参数<typeparamref name="TEntity"/>)</description>
    ///     </item>
    ///     <item>
    ///         <term>迭代：</term>
    ///         <description>将第二步中得到的对象，通过循环迭代，提供对外接口(<see cref="IAsyncEnumerator{E}.MoveNextAsync()"/>)</description>
    ///     </item>
    /// </list>
    /// 这三个任务将会作为一个阶段不断重复，直到抓取不到新的页面(抓取到的JSON资源<c>contents</c>部分为空或者HTTP请求失败)为止，此时视为迭代结束
    /// </summary>
    /// <remarks>
    /// 由于该类承担了多种任务，因此引入了两个泛型参数<typeparamref name="TEntity"/>和<typeparamref name="TRawEntity"/>，其中<typeparamref name="TEntity"/>代表
    /// 在第二阶段中所提到的简化后的统一对象类型，而<typeparamref name="TRawEntity"/>则指第一阶段中JSON反序列化的实体类类型
    /// </remarks>
    /// <typeparam name="TEntity">统一的对象类型</typeparam>
    /// <typeparam name="TRawEntity">JSON反序列化实体类类型</typeparam>
    public abstract class AbstractPixivAsyncEnumerator<TEntity, TRawEntity> : IAsyncEnumerator<TEntity?>
        where TEntity : class?
    {
        protected readonly IFetchEngine<TEntity>? PixivFetchEngine;

        protected IEnumerator<TEntity>? CurrentEntityEnumerator;

        public TEntity? Current => CurrentEntityEnumerator?.Current;

        /// <summary>
        /// 当前的任务采用了哪种<see cref="MakoApiKind"/>
        /// </summary>
        private readonly MakoApiKind _apiKind;

        protected readonly MakoClient MakoClient;
        
        /// <summary>
        /// 指示当前的任务是否已经被取消
        /// </summary>
        /// <remarks>
        /// 如果<see cref="PixivFetchEngine"/>是<c>null</c>，则直接返回<c>true</c>来中断任务执行
        /// </remarks>
        protected bool IsCancellationRequested => PixivFetchEngine?.IsCanceled ?? true;

        protected AbstractPixivAsyncEnumerator(IFetchEngine<TEntity>? pixivFetchEngine, MakoApiKind apiKind, MakoClient makoClient)
        {
            PixivFetchEngine = pixivFetchEngine;
            _apiKind = apiKind;
            MakoClient = makoClient;
        }

        public abstract ValueTask<bool> MoveNextAsync();

        /// <summary>
        /// 通过<paramref name="rawEntity"/>更新<see cref="CurrentEntityEnumerator"/>
        /// </summary>
        /// <param name="rawEntity">要使用到的<typeparamref name="TRawEntity"/></param>
        protected abstract void Update(TRawEntity rawEntity);

        /// <summary>
        /// 测试对应的JSON实体对象是否合法，比如，是否是<c>null</c>，是否缺失关键字段
        /// </summary>
        /// <param name="rawEntity">要被测试的JSON实体</param>
        /// <returns>JSON实体对象是否合法</returns>
        protected abstract bool ValidateResponse(TRawEntity rawEntity);

        protected async Task<Result<TRawEntity>> GetJsonResponse(string url)
        {
            var result = await MakoClient.GetMakoTaggedHttpClient(_apiKind).GetFromJsonAsync<TRawEntity>(url);
            if (result is null) return Result<TRawEntity>.Failure;
            return ValidateResponse(result)
                ? Result<TRawEntity>.Success(result)
                : Result<TRawEntity>.Failure;
        }
        
        public ValueTask DisposeAsync() => default;
    }
}