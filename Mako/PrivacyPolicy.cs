using JetBrains.Annotations;
using Mako.Util;

namespace Mako
{
    [PublicAPI]
    public enum PrivacyPolicy
    {
        [Description("public")]
        Public, 
        
        [Description("private")]
        Private
    }
}