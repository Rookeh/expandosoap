# ExpandoSoap 🧼

ExpandoSoap is a work-in-progress .NET SOAP client library that was created - mostly as an experiment - to allow SOAP interfaces to be built and called at runtime, without having to first generate proxy classes from WSDL and compile these into your application.

It uses the System.Dynamic type to allow function names and return types of the service to be handled at runtime.

The only requirement for strongly-typed data is for any input required by your SOAP service, and (optionally) any output provided in response.

## Usage

### Constructing an ExpandoSoap Client:

ExpandoSoap clients are created using the `ExpandoSoapFactory` static class which is referenced from the `ExpandoSoap.Factories` namespace.

Clients can be created by the factory from either a `Uri` which points to the WSDL of a web service, or from a `string` which contains the WSDL itself.

#### From a WSDL Uri:

```csharp
    Uri wsdlUri = new Uri("http://example.org/mySoapService?wsdl");
    dynamic client = await ExpandoSoapFactory.CreateClientAsync(wsdlUri);
```

**Note:** Because the WSDL content must first be retrieved from the remote server, this function is asynchronous.

#### From a WSDL string:

```csharp
    string wsdlString = "<definitions name=..."; // e.g. Read from a file.
    dynamic client = ExpandoSoapFactory.CreateClient(wsdlString);
```

### Using the Client object:

Because the client object is dynamic, the names of the functions to be called are written before compile time, but they are resolved into actual functions at runtime.

Consider the following (simplified) WSDL which defines a service to add movies to a library:

```xml
   <wsdl:types>
      <xsd:schema targetNamespace="http://example.org/MovieService">
        <xsd:element name="Movie">
          <xsd:complexType>
            <xsd:sequence>
              <xsd:element name="ID" type="xsd:string" minOccurs="0"/>
              <xsd:element name="Title" type="xsd:string"/>
              <xsd:element name="Director" type="xsd:string"/>
            </xsd:sequence>
          </xsd:complexType>
        </xsd:element>
        <xsd:element name="AddMovie">
          <xsd:complexType>
            <xsd:sequence>
              <xsd:element ref="tns:Movie" minOccurs="1" maxOccurs="1"/>
            </xsd:sequence>
          </xsd:complexType>
        </xsd:element>
      </xsd:schema>
    </wsdl:types>
    <wsdl:message name="AddMovieRequest">
      <wsdl:part name="parameters" element="tns:AddMovie"></wsdl:part>
    </wsdl:message>
    <wsdl:portType name="MovieService">
      <wsdl:operation name="AddMovie">
        <wsdl:input message="tns:AddMovieRequest"></wsdl:input>
      </wsdl:operation>
    </wsdl:portType>
```

In this scenario, if we wanted to call the `AddMovie` service, we would use the following syntax:

```csharp
    Movie movie = new Movie {Id = 1, Title = "Example", Director = "Me"};
    
    Uri wsdlUri = new Uri("http://example.org/MovieService?wsdl");
    dynamic client = await ExpandoSoapFactory.CreateClientAsync(wsdlUri);
    ExpandoObject soapResult = await client.AddMovie(movie);
```    

**Note:** All client functions are asynchronous.

Because this SOAP function takes an input parameter (`movie`), we must define a C# class (decorated with the appropriate XML namespace) representing this type:

```csharp    
    [XmlRoot(Namespace = "http://example.org/MovieService")]
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
    }
```

If your SOAP function takes no input parameter, you must pass `null` instead.

### Response Object

The `soapResult` object returned by the client is an `ExpandoObject` which represents the service's response to the request.

It can be used as-is, or converted back into a strong type using the provided `ConvertTo<T>` extension method from `ExpandoSoap.Extensions`:

```csharp
    ExpandoObject soapResponse = await client.GetMovie(args);
    Movie movie = soapResponse.ConvertTo<Movie>();
```

`ConvertTo<T>` will traverse the response XML until it finds an element with a name (ignoring case and namespace prefix) that matches the name of the provided type, and it will then attempt to construct an instance of that type with properties equal to the value of each of the child elements of the matching parent element. In the case of complex types, the function will call itself recursively until all nested properties have been constructed.

If a type cannot be converted for any reason, an exception will be thrown by the `ConvertTo` method, which should be handled appropriately. If a given property exists in the provided type but not the response XML, this property will be `null`.


## Compatibility

ExpandoSoap is very much a work in progress and probably not suitable for any kind of production use (yet). 

It has been tested with a few SOAP/WCF service implementations; however, given the following constraints:

- There is a lot of variation between how SOAP services are implemented
- There are almost certainly edge cases that have not been considered

...there is no guarantee that ExpandoSoap will function correctly with any given service.

If you run into a problem using ExpandoSoap with a particular SOAP service, feel free to open an issue and include the following information:

 - Observed behaviour and steps to reproduce.
 - Exception (if any) thrown by ExpandoSoap.
 - (If possible & permissible) WSDL for the SOAP service.
 - (If possible & permissible) URL of the SOAP service.
