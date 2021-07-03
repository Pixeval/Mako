using JetBrains.Annotations;

namespace Mako.Engines
{
    [PublicAPI]
    public interface INotifyCompletion
    {
        bool IsCompleted { get; set; }
    }
}