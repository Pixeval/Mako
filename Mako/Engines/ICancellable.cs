using JetBrains.Annotations;

namespace Mako.Engines
{
    /// <summary>
    /// 表示一个可以被取消的任务
    /// </summary>
    [PublicAPI]
    public interface ICancellable
    {
        bool IsCanceled { get; set; }
    }
}