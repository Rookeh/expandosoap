using ExpandoSoap.Functions;
using System.Collections.Generic;

namespace ExpandoSoap.Interfaces
{
    internal interface ISoapFunctionFactory
    {
        IEnumerable<SOAPFunction> GetFunctionsForDefinitions(definitions definitions);
    }
}