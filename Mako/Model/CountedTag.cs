namespace Mako.Model
{
    public record CountedTag
    {
        public CountedTag(Tag tag, long count)
        {
            Tag = tag;
            Count = count;
        }

        public Tag Tag { get; }

        public long Count { get; }
    }
}