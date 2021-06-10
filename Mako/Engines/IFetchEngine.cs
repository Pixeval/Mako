using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mako.Engines
{
    /// <summary>
    /// 一个抽象的，针对相同类型对象的搜索引擎，本质上是一个<see cref="IAsyncEnumerable{E}"/>，提供了内容过滤和插入策略等功能
    /// <para>
    /// 为了保证UI的流畅度，该API使用边搜索边插入的策略，每搜索到一个新的元素就立即反馈给UI，而不是等到所有元素均搜索完毕后一并返回
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// IFetchEngine&lt;E&gt; engine = GetFetchEngine();
    /// await foreach (var element in engine)
    /// {
    ///     ProcessElement(element);
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="E">搜索引擎所搜索的对象类型</typeparam>
    [PublicAPI]
    public interface IFetchEngine<E> : IAsyncEnumerable<E>, ICancellable
    {
        /// <summary>
        /// 指示该引擎已经搜索了多少页，每页都会包含多个<see cref="E"/>实例
        /// </summary>
        int RequestedPages { get; set; }

        /// <summary>
        /// 把一个<typeparamref name="E"/>插入到<see cref="IList{E}"/>中
        /// 可以在这个方法里选择把<typeparamref name="E"/>插入到<see cref="IList{E}"/>
        /// 中的合适位置
        /// </summary>
        /// <param name="list">被插入的列表</param>
        /// <param name="item">要插入的元素</param>
        void InsertTo(IList<E> list, E? item);

        /// <summary>
        /// 验证一个<typeparamref name="E"/>是否有资格被放到<see cref="IList{E}"/>中
        /// 可以在这个方法里验证<paramref name="list"/>是否已经包含<paramref name="item"/>
        /// </summary>
        /// <param name="list">要被插入的列表</param>
        /// <param name="item">要检查的元素</param>
        /// <returns>该元素是否可以被放进<paramref name="list"/>中</returns>
        bool Validate(IList<E> list, E? item);
    }
}