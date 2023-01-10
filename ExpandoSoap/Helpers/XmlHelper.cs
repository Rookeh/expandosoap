using System.IO;
using System.Text;
using System.Xml;

namespace ExpandoSoap.Helpers
{
    internal static class XmlHelper
    {
        internal static string PrettyPrint(string xml)
        {
            var stream = new MemoryStream();
            var textWriter = new XmlTextWriter(stream, Encoding.Unicode);
            var document = new XmlDocument();

            try
            {
                document.LoadXml(xml);
                textWriter.Formatting = Formatting.Indented;
                document.WriteContentTo(textWriter);
                textWriter.Flush();
                stream.Flush();
                stream.Position = 0;
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (XmlException)
            {
                return xml;
            }
            finally
            {
                stream.Close();
                textWriter.Close();
            }
        }
    }
}