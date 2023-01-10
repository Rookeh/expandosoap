using System.IO;
using System.Xml.Serialization;

namespace ExpandoSoap.WSDL
{
    internal static class WsdlParser
    {
        public static definitions Parse(string wsdl)
        {
            var serializer = new XmlSerializer(typeof(definitions));
            definitions definitions;

            using (var reader = new StringReader(wsdl))
            {
                definitions = (definitions)serializer.Deserialize(reader);
            }

            return definitions;
        }
    }
}