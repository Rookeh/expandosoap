using ExpandoSoap.WSDL;
using System.IO;
using Xunit;

namespace ExpandoSoap.Tests
{
    public class WsdlParserTests
    {
        [Fact]
        public void Parse_GivenWsdlString_ReturnsDefinitionsObject()
        {
            // Arrange
            var wsdl = File.ReadAllText(@"TestData\Example.wsdl");

            // Act
            var definitions = WsdlParser.Parse(wsdl);

            // Assert
            Assert.NotNull(definitions);
        }
    }
}
