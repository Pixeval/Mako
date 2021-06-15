using JetBrains.Annotations;

namespace Mako.Engines
{
    [PublicAPI]
    public interface ICompletionNotify
    {
        void Complete();
    }
}