using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Mako.Preference
{
    [PublicAPI]
    public interface ISessionUpdate
    {
        Task<Session> RefreshAsync(MakoClient makoClient);
    }
}