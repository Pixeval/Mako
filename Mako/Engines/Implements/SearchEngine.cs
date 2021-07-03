using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Net;
using Mako.Util;

namespace Mako.Engines.Implements
{
    internal class SearchEngine : AbstractPixivFetchEngine<Illustration>
    {
        private readonly int _current;
        private readonly DateTime? _endDate;
        private readonly SearchTagMatchOption _matchOption;
        private readonly int _pages;
        private readonly SearchDuration? _searchDuration;
        private readonly IllustrationSortOption _sortOption;
        private readonly DateTime? _startDate;
        private readonly string _tag;
        private readonly TargetFilter _targetFilter;
        
        public SearchEngine(
            MakoClient makoClient,
            EngineHandle? engineHandle,
            SearchTagMatchOption matchOption,
            string tag,
            int start,
            int pages,
            IllustrationSortOption? sortOption,
            SearchDuration? searchDuration,
            DateTime? startDate,
            DateTime? endDate,
            TargetFilter? targetFilter) : base(makoClient, engineHandle)
        {
            _matchOption = matchOption;
            _tag = tag;
            _current = start;
            _pages = pages;
            _sortOption = sortOption ?? IllustrationSortOption.PublishDateDescending;
            _searchDuration = searchDuration;
            _startDate = startDate;
            _endDate = endDate;
            _targetFilter = targetFilter ?? TargetFilter.ForAndroid;
        }

        public override IAsyncEnumerator<Illustration> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new SearchAsyncEnumerator(this, MakoApiKind.AppApi)!;
        }

        private class SearchAsyncEnumerator : RecursivePixivAsyncEnumerators.Illustration<SearchEngine>
        {
            public SearchAsyncEnumerator([NotNull] SearchEngine pixivFetchEngine, MakoApiKind makoApiKind) : base(pixivFetchEngine, makoApiKind)
            {
            }

            protected override string InitialUrl()
            {
                return GetSearchUrl();
            }

            protected override bool HasNextPage()
            {
                return PixivFetchEngine.RequestedPages <= PixivFetchEngine._pages - 1;
            }

            private string GetSearchUrl()
            {
                var match = PixivFetchEngine._matchOption.GetDescription();
                var startDateSegment = PixivFetchEngine._startDate?.Let(dn => $"&start_date={dn:yyyy-MM-dd}");
                var endDateSegment = PixivFetchEngine._endDate?.Let(dn => $"&start_date={dn:yyyy-MM-dd}");
                var durationSegment = PixivFetchEngine._searchDuration?.Let(du => $"&duration={du.GetDescription()}");
                return $"/v1/search/illust?search_target={match}&word={PixivFetchEngine._tag}&filter={PixivFetchEngine._targetFilter.GetDescription()}&offset={PixivFetchEngine._current}&sort={PixivFetchEngine._sortOption.GetDescription()}{startDateSegment}{endDateSegment}{durationSegment}";
            }
        }
    }
}