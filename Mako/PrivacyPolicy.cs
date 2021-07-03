using JetBrains.Annotations;
using Mako.Util;

namespace Mako
{
    /// <summary>
    ///     The privacy policy of Pixiv, be aware that the <see cref="Private" /> option
    ///     is only permitted when the ID is pointing to yourself
    /// </summary>
    [PublicAPI]
    public enum PrivacyPolicy
    {
        [Description("public")]
        Public,

        [Description("private")]
        Private
    }
}