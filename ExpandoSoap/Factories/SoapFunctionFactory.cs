using Newtonsoft.Json;
using ExpandoSoap.Extensions;
using ExpandoSoap.Functions;
using ExpandoSoap.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("ExpandoSoap.Tests")]
namespace ExpandoSoap.Factories
{
    internal class SoapFunctionFactory : ISoapFunctionFactory
    {
        private readonly IHttpClient _httpClient;

        public SoapFunctionFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IEnumerable<SOAPFunction> GetFunctionsForDefinitions(definitions definitions)
        {
            foreach (var binding in definitions.binding)
            {
                foreach (var operation in binding.operation)
                {
                    var functionName = operation.name;
                    var nsUrl = definitions.targetNamespace;
                    var nsPrefix = binding.type.Substring(0, binding.type.IndexOf(':'));
                    var soapUrl = definitions.service.SelectMany(s => s.port)
                        .FirstOrDefault(p => 
                            p.name.Contains(binding.type.Substring(binding.type.IndexOf(':') + 1, binding.type.Length - binding.type.IndexOf(':') - 1)) ||
                            binding.type.Substring(binding.type.IndexOf(':') + 1, binding.type.Length - binding.type.IndexOf(':') - 1).Contains(p.name)
                         )
                        ?.address
                        ?.location;

                    if (string.IsNullOrEmpty(soapUrl))
                    {
                        throw new InvalidOperationException("Unable to locate SOAP service URL.");
                    }

                    yield return new SOAPFunction
                    {
                        Name = functionName,
                        Function = new Func<object, Task<dynamic>>(async input =>
                        {
                            using (var sw = new StringWriter())
                            {
                                string soapEnvelope;

                                if (input != null)
                                {                                    
                                    var inputType = input.GetType();
                                    var serializer = new XmlSerializer(inputType);
                                    var ns = new XmlSerializerNamespaces();
                                    ns.Add(nsPrefix, nsUrl);
                                    serializer.Serialize(sw, input, ns);
                                    var body = sw.ToString();
                                    string element = GetMessageElement(definitions, operation);
                                    soapEnvelope = body.CreateSoapEnvelope(element, nsPrefix, nsUrl);
                                }
                                else
                                {
                                    string element = GetMessageElement(definitions, operation);
                                    soapEnvelope = element.CreateSoapEnvelopeWithoutArguments(nsPrefix, nsUrl);
                                }
                                
                                var stringContent = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
                                var response = await _httpClient.PostAsync(soapUrl, stringContent);
                                var responseBody = await response.Content.ReadAsStringAsync();

                                if (!response.IsSuccessStatusCode)
                                {
                                    throw new InvalidOperationException($"Failed to call SOAP function {functionName}: Response code was {response.StatusCode}. Response message: {responseBody}");
                                }
                                else
                                {
                                    try
                                    {
                                        var responseDoc = XDocument.Parse(responseBody);
                                        string jsonText = JsonConvert.SerializeXNode(responseDoc);
                                        var expando = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);
                                        var envelope = ((IDictionary<string, object>)expando)["soapenv:Envelope"];
                                        return ((IDictionary<string, object>)envelope)["soapenv:Body"];
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new InvalidOperationException($"Failed to deserialize SOAP response from function {functionName}: {ex.Message}", ex);
                                    }
                                }
                            }
                        })
                    };
                }
            }
        }

        private static string GetMessageElement(definitions definitions, operation operation)
        {
            var inputs = definitions.portType.SelectMany(p => p.operation)
                                                    .Single(o => o.name == operation.name)
                                                    .input;

            var messagePart = definitions.message
                .SingleOrDefault(m => inputs.Any(i => i.message.Contains(m.name)))?.part
                .SingleOrDefault();

            var element = messagePart.element;
            if (element.Contains(':'))
            {
                element = element.Substring(element.IndexOf(':') + 1, element.Length - element.IndexOf(':') - 1);
            }

            return element;
        }
    }
}