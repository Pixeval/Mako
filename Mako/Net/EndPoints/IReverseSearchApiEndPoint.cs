// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Mako.Net.Request;
using Mako.Net.Response;
using WebApiClientCore.Attributes;
using WebApiClientCore.Parameters;

namespace Mako.Net.EndPoints;

[HttpHost("https://saucenao.com/")]
internal interface IReverseSearchApiEndPoint
{
    [HttpPost("/search.php")]
    Task<ReverseSearchResponse> GetSauceAsync([FormDataContent] FormDataFile file, [PathQuery] ReverseSearchRequest request);
}
