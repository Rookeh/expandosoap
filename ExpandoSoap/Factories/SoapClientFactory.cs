using ExpandoSoap.Interfaces;
using ExpandoSoap.WSDL;
using System.Collections.Generic;
using System.Dynamic;

namespace ExpandoSoap.Factories
{
    internal class SoapClientFactory : ISoapClientFactory
    {
        private readonly ISoapFunctionFactory _soapFunctionFactory;

        public SoapClientFactory(ISoapFunctionFactory soapFunctionFactory)
        {
            _soapFunctionFactory = soapFunctionFactory;
        }

        public dynamic BuildSoapClient(string wsdl)
        {
            var definitions = WsdlParser.Parse(wsdl);
            var functions = _soapFunctionFactory.GetFunctionsForDefinitions(definitions);

            dynamic expando = new ExpandoObject();

            foreach (var func in functions)
            {
                ((IDictionary<string, object>)expando)[func.Name] = func.Function;
            }

            return expando;
        }
    }
}