using Moq;
using ExpandoSoap.Factories;
using ExpandoSoap.Interfaces;
using System.IO;
using Xunit;

namespace ExpandoSoap.Tests
{
    public class SoapClientFactoryTests
    {
        private readonly Mock<IHttpClient> _mockHttpClient;
        private readonly SoapClientFactory _factory;

        public SoapClientFactoryTests()
        {
            _mockHttpClient = new Mock<IHttpClient>();

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(x => x.CreateClient())
                .Returns(_mockHttpClient.Object);

            var functionFactory = new SoapFunctionFactory(mockHttpClientFactory.Object);

            _factory = new SoapClientFactory(functionFactory);
        }

        [Fact]
        public void BuildSoapClass_GivenWsdl_ConstructsDynamicSoapClass()
        {
            // Arrange
            var wsdl = File.ReadAllText(@"TestData\Example.wsdl");

            // Act
            var soapClient = _factory.BuildSoapClient(wsdl);

            // Assert
            Assert.NotNull(soapClient);
            Assert.NotNull(soapClient.AddMovie);
        }
    }
}