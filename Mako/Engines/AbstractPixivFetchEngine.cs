using System;
using System.Collections.Generic;
using System.Threading;

namespace Mako.Engines
{
    
    /// <summary>
    /// 一个针对Pixiv所特化的<see cref="IFetchEngine{E}"/>，提供了<see cref="IFetchEngine{E}.InsertTo"/>
    /// 以及<see cref="IFetchEngine{E}.Validate"/>的默认行为
    /// </summary>
    /// <typeparam name="E"><inheritdoc cref="IFetchEngine{E}"/></typeparam>
    internal abstract class AbstractPixivFetchEngine<E> : IFetchEngine<E>
    {
        protected AbstractPixivFetchEngine(EngineHandle? engineHandle)
        {
            EngineHandle = engineHandle ?? new EngineHandle(Guid.NewGuid().ToString());
        }

        public abstract IAsyncEnumerator<E> GetAsyncEnumerator(CancellationToken cancellationToken = new());

        public abstract MakoClient MakoClient { get; set; }
        
        public int RequestedPages { get; set; }
        
        /// <summary>
        /// <see cref="IFetchEngine{E}.InsertTo"/>的默认行为，如果<paramref name="item"/>
        /// 不为<c>null</c>就将其放入<paramref name="list"/>中
        /// </summary>
        /// <param name="list"><inheritdoc cref="IFetchEngine{E}.InsertTo"/></param>
        /// <param name="item"><inheritdoc cref="IFetchEngine{E}.InsertTo"/></param>
        public virtual void InsertTo(IList<E> list, E? item)
        {
            if (item is not null)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// <see cref="IFetchEngine{E}.Validate"/>的默认行为，仅检测<paramref name="item"/>
        /// 是否为<c>null</c>且<paramref name="item"/>是否已经在<paramref name="list"/>中
        /// </summary>
        /// <param name="list"><inheritdoc cref="IFetchEngine{E}.Validate"/></param>
        /// <param name="item"><inheritdoc cref="IFetchEngine{E}.Validate"/></param>
        /// <returns><inheritdoc cref="IFetchEngine{E}.Validate"/></returns>
        public virtual bool Validate(IList<E> list, E? item)
        {
            return item is not null && list.Contains(item);
        }
        
        public EngineHandle EngineHandle { get; }
    }
}