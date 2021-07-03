using System.Net;
using System.Threading.Tasks;

namespace Mako.Net
{
    internal interface INameResolver
    {
        Task<IPAddress[]> Lookup(string hostname);
    }
}