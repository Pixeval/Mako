using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines.Implements
{
    public class UserSearchEngine : AbstractPixivFetchEngine<User>
    {
        private readonly string _keyword;
        private readonly TargetFilter _targetFilter;
        private readonly UserSortOption _userSortOption;
        
        public UserSearchEngine([NotNull] MakoClient makoClient, TargetFilter targetFilter, UserSortOption? userSortOption, string keyword, EngineHandle? engineHandle) : base(makoClient, engineHandle)
        {
            _keyword = keyword;
            _targetFilter = targetFilter;
            _userSortOption = userSortOption ?? UserSortOption.DateDescending;
        }

        public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new UserSearchAsyncEnumerator(this, MakoApiKind.AppApi)!;
        }

        private class UserSearchAsyncEnumerator : RecursivePixivAsyncEnumerators.User<UserSearchEngine>
        {
            public UserSearchAsyncEnumerator([NotNull] UserSearchEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override string InitialUrl()
            {
                return $"https://app-api.pixiv.net/v1/search/user?filter={PixivFetchEngine._targetFilter.GetDescription()}&word={PixivFetchEngine._keyword}&sort={PixivFetchEngine._userSortOption.GetDescription()}";
            }
        }
    }
}