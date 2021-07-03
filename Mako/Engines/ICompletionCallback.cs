using JetBrains.Annotations;

namespace Mako.Engines
{
    [PublicAPI]
    public interface ICompletionCallback<in T>
    {
        void OnCompletion(T param);
    }
}