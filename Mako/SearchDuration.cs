using JetBrains.Annotations;
using Mako.Util;

namespace Mako
{
    [PublicAPI]
    public enum SearchDuration
    {
        [Description("within_last_day")]
        WithinLastDay,
        
        [Description("within_last_week")]
        WithinLastWeek,
        
        [Description("within_last_month")]
        WithinLastMonth,
    }
}