using System;

namespace ExpandoSoap.Functions
{
    internal class SOAPFunction
    {
        public string Name { get; set; }
        public Func<dynamic, dynamic> Function { get; set; }
    }
}