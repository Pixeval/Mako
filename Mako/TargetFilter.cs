using JetBrains.Annotations;
using Mako.Util;

namespace Mako
{
    [PublicAPI]
    public enum TargetFilter
    {
        [Description("for_android")]
        ForAndroid,

        [Description("for_ios")]
        ForIos
    }
}