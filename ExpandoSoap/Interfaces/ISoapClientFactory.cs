namespace ExpandoSoap.Interfaces
{
    internal interface ISoapClientFactory
    {
        dynamic BuildSoapClient(string wsdl);
    }
}