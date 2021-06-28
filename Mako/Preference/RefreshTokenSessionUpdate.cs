using System.Threading.Tasks;
using Mako.Net.EndPoints;
using Mako.Net.Request;
using Mako.Util;

namespace Mako.Preference
{
    public class RefreshTokenSessionUpdate : ISessionUpdate
    {
        public async Task<Session> Refresh(MakoClient makoClient)
        {
            return (await makoClient.Resolve<IAuthEndPoint>().Refresh(new RefreshSessionRequest(makoClient.Session.RefreshToken)))
                .ToSession().With(makoClient.Session);
        }
    }
}