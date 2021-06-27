using System.Collections.Generic;
using System.Net.Http;
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
    /// <see cref="AbstractPixivAsyncEnumerator{TEntity,TRawEntity,TFetchEngine}"/>提供了一个枚举器以枚举JSON资源的
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
    /// <typeparam name="TFetchEngine">搜索引擎类型</typeparam>
    public abstract class AbstractPixivAsyncEnumerator<TEntity, TRawEntity, TFetchEngine> : IAsyncEnumerator<TEntity?>
        where TEntity : class?
        where TFetchEngine : class, IFetchEngine<TEntity>
    {
        /// <summary>
        /// 本枚举器所属的<see cref="IFetchEngine{E}"/>
        /// </summary>
        protected readonly TFetchEngine PixivFetchEngine;

        /// <summary>
        /// 当前搜索页面的结果列表的迭代器
        /// </summary>
        protected IEnumerator<TEntity>? CurrentEntityEnumerator;

        /// <summary>
        /// 当前迭代到的结果
        /// </summary>
        public TEntity? Current => CurrentEntityEnumerator?.Current;

        /// <summary>
        /// 当前的任务采用了哪种<see cref="MakoApiKind"/>
        /// </summary>
        private MakoApiKind ApiKind { get; }

        /// <summary>
        /// 本枚举器所属的<see cref="MakoClient"/>
        /// </summary>
        protected readonly MakoClient MakoClient;
        
        /// <summary>
        /// 指示当前的任务是否已经被取消
        /// </summary>
        /// <remarks>
        /// 如果<see cref="PixivFetchEngine"/>是<c>null</c>，则直接返回<c>true</c>来中断任务执行
        /// </remarks>
        protected bool IsCancellationRequested => PixivFetchEngine.EngineHandle.CancellationTokenSource.IsCancellationRequested;

        protected AbstractPixivAsyncEnumerator(TFetchEngine pixivFetchEngine, MakoApiKind apiKind)
        {
            PixivFetchEngine = pixivFetchEngine;
            MakoClient = pixivFetchEngine.MakoClient;
            ApiKind = apiKind;
        }

        public abstract ValueTask<bool> MoveNextAsync();

        /// <summary>
        /// 测试对应的JSON实体对象是否合法，比如，是否是<c>null</c>，是否缺失关键字段
        /// </summary>
        /// <param name="rawEntity">要被测试的JSON实体</param>
        /// <returns>JSON实体对象是否合法</returns>
        protected abstract bool ValidateResponse(TRawEntity rawEntity);

        /// <summary>
        /// 从<paramref name="url"/>获取对应的JSON结果
        /// </summary>
        /// <param name="url">要请求的URL</param>
        /// <returns>标志着请求结果的<see cref="Task{TResult}"/></returns>
        /// <exception cref="MakoNetworkException">如果请求过程中抛出<see cref="HttpRequestException"/>或者响应码标志着请求失败</exception>
        protected async Task<Result<TRawEntity>> GetJsonResponse(string url)
        {
            try
            {
                var responseMessage = await MakoClient.ResolveKeyed<HttpClient>(ApiKind).GetAsync(url);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    return Result<TRawEntity>.OfFailure(await MakoNetworkException.FromHttpResponseMessage(responseMessage, MakoClient.Configuration.Bypass));
                }

                var result = (await responseMessage.Content.ReadAsStringAsync()).FromJson<TRawEntity>();
                if (result is null) return Result<TRawEntity>.OfFailure();
                return ValidateResponse(result)
                    ? Result<TRawEntity>.OfSuccess(result)
                    : Result<TRawEntity>.OfFailure();
            }
            catch (HttpRequestException e)
            {
                return Result<TRawEntity>.OfFailure(new MakoNetworkException(url, MakoClient.Configuration.Bypass, e.Message, (int?) e.StatusCode ?? -1));
            }
        }
        
        public ValueTask DisposeAsync() => default;
    }
}