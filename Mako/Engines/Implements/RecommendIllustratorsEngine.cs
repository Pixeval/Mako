using System.Collections.Generic;
using System.Threading;
using Mako.Model;

namespace Mako.Engines.Implements
{
    internal class RecommendIllustratorsEngine : AbstractPixivFetchEngine<User>
    {
        public sealed override MakoClient MakoClient { get; set; }
        
        public RecommendIllustratorsEngine(EngineHandle? engineHandle, MakoClient makoClient) : base(engineHandle)
        {
            MakoClient = makoClient;
        }

        public override IAsyncEnumerator<User> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            throw new System.NotImplementedException();
        }
    }
}