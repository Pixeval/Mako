using JetBrains.Annotations;

namespace Mako.Net
{
    /// <summary>
    ///     The several kinds of APIs that Mako will use
    /// </summary>
    [PublicAPI]
    public enum MakoApiKind
    {
        AppApi,
        WebApi,
        AuthApi,
        ImageApi
    }
}