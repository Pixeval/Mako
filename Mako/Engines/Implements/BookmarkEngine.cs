using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Net.Response;
using Mako.Util;

namespace Mako.Engines.Implements
{
    /// <summary>
    /// 获取用户收藏的Pixiv搜素引擎
    /// </summary>
    internal class BookmarkEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly string _uid;
        private readonly PrivacyPolicy _privacyPolicy;
        private readonly TargetFilter _targetFilter;

        /// <summary>
        /// 创建一个新的<see cref="BookmarkEngine"/>
        /// </summary>
        /// <param name="makoClient">该<see cref="BookmarkEngine"/>所属的<see cref="MakoClient"/></param>
        /// <param name="uid">要搜索的作者ID</param>
        /// <param name="privacyPolicy">收藏的隐私策略</param>
        /// <param name="targetFilter"></param>
        /// <param name="engineHandle">该实例的<see cref="EngineHandle"/>句柄，如果设置为<c>null</c>则会自动创建</param>
        public BookmarkEngine(
            MakoClient makoClient,
            string uid, 
            PrivacyPolicy privacyPolicy,
            TargetFilter targetFilter,
            EngineHandle? engineHandle = null) : base(makoClient, engineHandle)
        {
            _uid = uid;
            _privacyPolicy = privacyPolicy;
            _targetFilter = targetFilter;
        }
        
        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new BookmarkAsyncEnumerator(this)!;
        }

        private class BookmarkAsyncEnumerator : RecursivePixivAsyncEnumerator<Illustration, PixivResponse, BookmarkEngine>
        {
            public BookmarkAsyncEnumerator(BookmarkEngine pixivFetchEngine) 
                : base(pixivFetchEngine, MakoApiKind.AppApi)
            {
            }
            
            protected override bool ValidateResponse(PixivResponse rawEntity)
            {
                return rawEntity.Illusts.IsNotNullOrEmpty();
            }

            protected override string? NextUrl(PixivResponse? rawEntity)
            {
                return rawEntity?.NextUrl;
            }

            protected override string InitialUrl()
            {
                return $"/v1/user/bookmarks/illust?user_id={PixivFetchEngine._uid}&restrict={PixivFetchEngine._privacyPolicy.GetDescription()}&filter={PixivFetchEngine._targetFilter.GetDescription()}";
            }

            protected override IEnumerator<Illustration>? GetNewEnumerator(PixivResponse? rawEntity)
            {
                return rawEntity?.Illusts?.SelectNotNull(MakoExtension.ToIllustration).GetEnumerator();
            }
        }
    }
}