#region Copyright (c) Pixeval/Mako

// MIT License
// 
// Copyright (c) Pixeval 2021 Mako/IAppApiEndPoint.cs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System.Threading.Tasks;
using Mako.Net.Request;
using Mako.Net.Response;
using Refit;

namespace Mako.Net.EndPoints
{
    internal interface IAppApiEndPoint
    {
        [Post("/v2/illust/bookmark/add")]
        Task AddBookmark([Body(BodySerializationMethod.UrlEncoded)] AddBookmarkRequest request);

        [Post("/v1/illust/bookmark/delete")]
        Task RemoveBookmark([Body(BodySerializationMethod.UrlEncoded)] RemoveBookmarkRequest request);

        [Get("/v1/illust/detail")]
        Task<PixivSingleIllustResponse> GetSingle([AliasAs("illust_id")] string id);

        [Get("/v1/user/detail")]
        Task<PixivSingleUserResponse> GetSingleUser(SingleUserRequest request);

        [Post("/v1/user/follow/add")]
        Task FollowUser([Body(BodySerializationMethod.UrlEncoded)] FollowUserRequest request);

        [Post("/v1/user/follow/delete")]
        Task RemoveFollowUser([Body(BodySerializationMethod.UrlEncoded)] RemoveFollowUserRequest request);

        [Get("/v1/trending-tags/illust")]
        Task<TrendingTagResponse> GetTrendingTags([AliasAs("filter")] string filter);
    }
}