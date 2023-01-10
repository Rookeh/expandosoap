using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace ExpandoSoap.Interfaces
{
    internal interface IHttpClientFactory
    {
        IHttpClient CreateClient();
    }
}