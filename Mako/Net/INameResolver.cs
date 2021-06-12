using System.Net;
using System.Threading.Tasks;

namespace Mako.Net
{
    /// <summary>
    /// 一个自定义的DNS，根据特定的Host解析对应的IP地址
    /// </summary>
    internal interface INameResolver
    {
        Task<IPAddress[]> Lookup(string hostname);
    }
}