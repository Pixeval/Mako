using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mako.Model
{
    [PublicAPI]
    public record User
    {
        public string? Name { get; set; }
        
        public string? Id { get; set; }
        
        public bool IsFollowed { get; set; }
        
        public string? Avatar { get; set; }
        
        public string? Introduction { get; set; }
        
        public int Follows { get; set; }
        
        public bool IsPremium { get; set; }
        
        public IEnumerable<string>? Thumbnails { get; set; }
    }
}