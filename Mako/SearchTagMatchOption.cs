using JetBrains.Annotations;
using Mako.Util;

namespace Mako
{
    [PublicAPI]
    public enum SearchTagMatchOption
    {
        [Description("partial_match_for_tags")]
        PartialMatchForTags,

        [Description("exact_match_for_tags")]
        ExactMatchForTags,

        [Description("title_and_caption")]
        TitleAndCaption
    }
}