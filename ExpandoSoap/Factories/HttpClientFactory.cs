using ExpandoSoap.Interfaces;
using ExpandoSoap.Wrappers;

namespace ExpandoSoap.Factories
{
    internal class HttpClientFactory : IHttpClientFactory
    {
        public IHttpClient CreateClient()
        {
            return new HttpClientWrapper();
        }
    }
}