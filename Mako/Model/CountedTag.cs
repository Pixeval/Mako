namespace Mako.Model
{
    public record CountedTag
    {
        public Tag Tag { get; }
        
        public long Count { get; }

        public CountedTag(Tag tag, long count)
        {
            Tag = tag;
            Count = count;
        }
    }
}