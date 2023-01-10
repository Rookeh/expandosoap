using ExpandoSoap.Helpers;
using System;

namespace ExpandoSoap.Extensions
{
    internal static class StringExtensions
    {
        internal static string CreateSoapEnvelopeWithoutArguments(this string functionName, string nsPrefix, string nsUrl)
        {
            return XmlHelper.PrettyPrint($@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:{nsPrefix}=""{nsUrl}"">
                                              <soapenv:Header/>                             
                                              <soapenv:Body>                             
                                                  <{nsPrefix}:{functionName}/>                                                       
                                              </soapenv:Body>
                                            </soapenv:Envelope>");
        }

        internal static string CreateSoapEnvelope(this string body, string functionName, string nsPrefix, string nsUrl)
        {
            var trimmed = body.Substring(body.IndexOf('>') + 1, body.Length - (body.IndexOf('>') + 1))
                              .Replace($@" xmlns:{nsPrefix}=""{nsUrl}""", string.Empty);

            return XmlHelper.PrettyPrint($@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:{nsPrefix}=""{nsUrl}"">
                                              <soapenv:Header/>                             
                                              <soapenv:Body>                             
                                                  <{nsPrefix}:{functionName}>                             
                                                      {trimmed}
                                                  </{nsPrefix}:{functionName}>                             
                                              </soapenv:Body>
                                            </soapenv:Envelope>");
        }
    }
}