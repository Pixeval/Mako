using JetBrains.Annotations;

namespace Mako.Model
{
    [PublicAPI]
    public record Resolution
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}