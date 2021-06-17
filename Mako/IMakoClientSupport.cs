using JetBrains.Annotations;

namespace Mako
{
    [PublicAPI]
    public interface IMakoClientSupport
    {
        public MakoClient MakoClient { get; }
    }
}