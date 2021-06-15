using JetBrains.Annotations;

namespace Mako.Engines
{
    [PublicAPI]
    public interface IEngineHandleSource
    {
        EngineHandle EngineHandle { get; }
    }
}