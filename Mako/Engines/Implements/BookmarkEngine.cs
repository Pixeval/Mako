using System.Collections.Generic;
using System.Threading;
using Mako.Model;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines.Implements
{
    /// <summary>
    ///     An <see cref="IFetchEngine{E}" /> that fetches the bookmark of a specific user
    /// </summary>
    internal class BookmarkEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly PrivacyPolicy _privacyPolicy;
        private readonly TargetFilter _targetFilter;
        private readonly string _uid;

        /// <summary>
        ///     Creates a <see cref="BookmarkEngine" />
        /// </summary>
        /// <param name="makoClient">The <see cref="MakoClient" /> that owns this object</param>
        /// <param name="uid">Id of the user</param>
        /// <param name="privacyPolicy">The privacy option</param>
        /// <param name="targetFilter">Indicates the target API of the fetch operation</param>
        /// <param name="engineHandle"></param>
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
            return RecursivePixivAsyncEnumerators.Illustration<BookmarkEngine>.WithInitialUrl(this, MakoApiKind.AppApi,
                engine => $"/v1/user/bookmarks/illust?user_id={engine._uid}&restrict={engine._privacyPolicy.GetDescription()}&filter={engine._targetFilter.GetDescription()}")!;
        }
    }
}