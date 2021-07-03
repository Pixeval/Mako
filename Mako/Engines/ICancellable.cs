using System.Threading;
using JetBrains.Annotations;

namespace Mako.Engines
{
    [PublicAPI]
    public interface ICancellable
    {
        CancellationTokenSource CancellationTokenSource { get; set; }
    }
}