using JetBrains.Annotations;

namespace Mako
{
    [PublicAPI]
    public interface IMakoClientSupport
    {
        MakoClient MakoClient { get; }
    }
}