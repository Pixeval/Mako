using System.Threading.Tasks;
using Mako.Model;
using Mako.Net.Request;
using Refit;

namespace Mako.Net.EndPoints
{
    internal interface IAuthEndPoint
    {
        [Post("/auth/token")]
        Task<TokenResponse> Refresh([Body(BodySerializationMethod.UrlEncoded)] RefreshSessionRequest request);
    }
}