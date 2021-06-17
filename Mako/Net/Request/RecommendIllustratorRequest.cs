using Refit;

namespace Mako.Net.Request
{
    internal class RecommendIllustratorRequest
    {
        [AliasAs("filter")]
        public string Filter => "for_android";

        [AliasAs("offset")]
        public int Offset { get; set; } = 0;
    }
}