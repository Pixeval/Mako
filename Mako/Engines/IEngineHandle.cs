using JetBrains.Annotations;

namespace Mako.Engines
{
    /// <summary>
    ///     Represents a class that is capable of tracking its own lifetime, any class that
    ///     implements <see cref="IEngineHandleSource" /> must exposes an <see cref="EngineHandle" />
    ///     that can be used to cancel itself or report the completion
    /// </summary>
    [PublicAPI]
    public interface IEngineHandleSource
    {
        EngineHandle EngineHandle { get; }
    }
}