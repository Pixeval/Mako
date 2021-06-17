using System.Threading.Tasks;
using Mako.Net.Request;
using Mako.Net.Response;
using Refit;

namespace Mako.Net.Protocol
{
    internal interface IAppApiProtocol
    {
        [Get("/v1/user/recommended?filter=for_android")]
        Task<PixivUserResponse> GetRecommendIllustrators(RecommendIllustratorRequest request);
    }
}