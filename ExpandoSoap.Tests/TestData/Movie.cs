using System.Xml.Serialization;

namespace ExpandoSoap.Tests.TestData
{
    [XmlRoot(Namespace = "http://example.org/MovieService/")]
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
    }
}