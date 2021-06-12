using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines
{
    internal abstract class RecursivePixivAsyncEnumerator<TEntity, TRawEntity> : AbstractPixivAsyncEnumerator<TEntity, TRawEntity> 
        where TEntity : class
    {
        protected TRawEntity? Entity { get; private set; }

        protected RecursivePixivAsyncEnumerator([CanBeNull] IFetchEngine<TEntity>? pixivFetchEngine, MakoApiKind apiKind, [NotNull] MakoClient makoClient) 
            : base(pixivFetchEngine, apiKind, makoClient)
        {
        }

        protected abstract string NextUrl();

        protected abstract string InitialUrl();

        protected abstract IEnumerator<TEntity> GetNewEnumerator();

        protected virtual bool HasNextPage() => NextUrl().IsNotNullOrEmpty();

        protected virtual bool HasNext() => true;

        public override async ValueTask<bool> MoveNextAsync()
        {
            if (IsCancellationRequested || !HasNext())
            {
                return false;
            }

            if (Entity is null)
            {
                var first = InitialUrl();
                switch (await GetJsonResponse(first))
                {
                    case Result.Success<TRawEntity> (var raw): 
                        Update(raw);
                        break;
                    default: 
                        Errors.ThrowNetworkException(first, PixivFetchEngine!.RequestedPages, null, MakoClient.Session?.Bypass ?? false);
                        break;
                }
            }

            if (CurrentEntityEnumerator!.MoveNext())
            {
                return true;
            }

            if (!HasNextPage())
            {
                return false;
            }

            if (await GetJsonResponse(NextUrl()) is Result.Success<TRawEntity> (var value))
            {
                Update(value);
                return true;
            }

            return false;
        }

        protected override void Update(TRawEntity rawEntity)
        {
            Entity = rawEntity;
            CurrentEntityEnumerator = GetNewEnumerator();
            PixivFetchEngine!.RequestedPages++;
        }
    }
}