using ExpandoSoap.Interfaces;
using System;
using System.Threading.Tasks;

namespace ExpandoSoap.Factories
{
    public static class ExpandoSoapFactory
    {
        private static ISoapClientFactory _soapClientFactory;
        private static IHttpClient _httpClient;

        static ExpandoSoapFactory()
        {
            var httpClientFactory = new HttpClientFactory();
            var soapFunctionFactory = new SoapFunctionFactory(httpClientFactory);
            _httpClient = httpClientFactory.CreateClient();
            _soapClientFactory = new SoapClientFactory(soapFunctionFactory);
        }

        public static async Task<dynamic> CreateClientAsync(Uri wsdlUri)
        {
            try
            {
                var wsdl = await (await _httpClient.GetAsync(wsdlUri)).Content.ReadAsStringAsync();
                return _soapClientFactory.BuildSoapClient(wsdl);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to construct SOAP client from WSDL URI {wsdlUri}: {ex.Message}", ex);
            }
        }

        public static dynamic CreateClient(string wsdl)
        {
            return _soapClientFactory.BuildSoapClient(wsdl);
        }
    }
}
