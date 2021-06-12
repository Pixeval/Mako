using JetBrains.Annotations;

namespace Mako.Model
{
    [PublicAPI]
    public record Tag
    {
        public string? Name { get; init; }

        public string? TranslatedName { get; init; }
    }
}