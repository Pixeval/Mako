using System;
using JetBrains.Annotations;

namespace Mako.Engines
{
    /// <summary>
    /// 一个搜索引擎的句柄，可以根据该句柄来取消或结束一个正在运行的搜索引擎，并且可以在结束时执行回调
    /// </summary>
    [PublicAPI]
#pragma warning disable 660,661 // Object.Equals() and Object.GetHashCode() are not overwritten
    public struct EngineHandle : ICancellable, INotifyCompletion, ICompletionCallback<EngineHandle>
#pragma warning restore 660,661
    {
        private readonly Action<EngineHandle>? _onCompletion;

        public bool Equals(EngineHandle other)
        {
            return Id == other.Id && IsCanceled == other.IsCanceled;
        }

        /// <summary>
        /// 搜索引擎的唯一ID
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// 指示该句柄对应的搜索引擎是否已经被取消
        /// </summary>
        public bool IsCanceled { get; set; }
        
        /// <summary>
        /// 指示该句柄对应的搜索引擎是否已经结束运行
        /// </summary>
        
        public bool IsCompleted { get; set; }

        public EngineHandle(Guid id, Action<EngineHandle>? onCompletion = null)
        {
            _onCompletion = onCompletion;
            Id = id;
            IsCanceled = false;
            IsCompleted = false;
        }
        
        public EngineHandle(Action<EngineHandle> onCompletion)
        {
            _onCompletion = onCompletion;
            Id = Guid.NewGuid();
            IsCanceled = false;
            IsCompleted = false;
        }

        /// <summary>
        /// 取消该句柄对应的搜索引擎的运行
        /// </summary>
        public void Cancel() => IsCanceled = true;

        /// <summary>
        /// 设置该句柄对应的搜索引擎的状态为已完成，并执行注册的结束回调
        /// </summary>
        public void Complete()
        {
            IsCompleted = true;
            OnCompletion(this);
        }

        public static bool operator ==(EngineHandle lhs, EngineHandle rhs)
        {
            return lhs.Id == rhs.Id && lhs.IsCanceled == rhs.IsCanceled && lhs.IsCompleted == rhs.IsCompleted;
        }

        public static bool operator !=(EngineHandle lhs, EngineHandle rhs)
        {
            return !(lhs == rhs);
        }

        public void OnCompletion(EngineHandle engineHandle)
        {
            _onCompletion?.Invoke(engineHandle);
        }
    }
}