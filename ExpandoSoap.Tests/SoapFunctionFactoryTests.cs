using Moq;
using ExpandoSoap.Extensions;
using ExpandoSoap.Factories;
using ExpandoSoap.Interfaces;
using ExpandoSoap.Tests.TestData;
using ExpandoSoap.WSDL;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Dynamic;

namespace ExpandoSoap.Tests
{
    public class SoapFunctionFactoryTests
    {
        private readonly Mock<IHttpClient> _mockHttpClient;
        private readonly SoapFunctionFactory _factory;

        public SoapFunctionFactoryTests()
        {
            _mockHttpClient = new Mock<IHttpClient>();
            
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(x => x.CreateClient())
                .Returns(_mockHttpClient.Object);

            _factory = new SoapFunctionFactory(mockHttpClientFactory.Object);
        }

        [Fact]
        public async void GetFunctionsForDefinitions_GivenDefinitions_BuildsFunctions()
        {
            // Arrange
            var wsdlText = File.ReadAllText(@"TestData\Example.wsdl");
            var serviceResponse = File.ReadAllText(@"TestData\MockResponse.xml");
            var definitions = WsdlParser.Parse(wsdlText);
            var expectedUrl = "http://www.example.org/MovieService";

            _mockHttpClient.Setup(x => x.PostAsync(expectedUrl, It.IsAny<StringContent>()))
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    Content = new StringContent(serviceResponse)
                }))
                .Verifiable();

            // Act
            var functions = _factory.GetFunctionsForDefinitions(definitions);

            // Assert            
            Assert.NotEmpty(functions);
            Assert.Single(functions);
            var addMovie = functions.Single(f => f.Name == "AddMovie").Function;
            var Movie = new Movie { Director = "Me", Id = 1234, Title = "Test" };
            ExpandoObject response = await addMovie(Movie);
            Assert.NotNull(response);
            _mockHttpClient.Verify();
        }
    }
}