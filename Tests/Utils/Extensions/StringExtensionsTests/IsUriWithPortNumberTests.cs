using Announcarr.Utils.Extensions.String;
using Shouldly;

namespace Announcarr.Test.Utils.Extensions.StringExtensionsTests;

public class IsUriWithPortNumberTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("http://")]
    [InlineData("http://localhost:65536")]
    [InlineData("http://localhost:-1")]
    [InlineData("http://localhost:abc")]
    [InlineData("http://hello,world")]
    public void When_IsUriWithPortNumberCalled_Given_InvalidUri_Then_ReturnFalse(string? uri)
    {
        bool result = uri.IsUriWithPortNumber();
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("localhost")]
    [InlineData("http://localhost")]
    [InlineData("http://localhost/")]
    [InlineData("http://localhost/api/path")]
    [InlineData("example.com")]
    [InlineData("www.example.com")]
    [InlineData("http://example.com")]
    [InlineData("http://example.com/")]
    [InlineData("http://example.com/api/path")]
    [InlineData("ftp://example.com")]
    [InlineData("https://example.com")]
    [InlineData("localhost:5055")]
    [InlineData("example.com:5055")]
    [InlineData("www.example.com:5055")]
    // The last 3 cases fail because the Uri class consider the hostname ("localhost", "example.com") to be the scheme and the port number to be the actual host.
    // Weird shit, but that's actually valid (example, about:preferences is a valid url and the scheme is about). More about it here - https://www.rfc-editor.org/rfc/rfc3986#section-3.2
    public void When_IsUriWithPortNumberCalled_Given_UriWithoutPortNumber_Then_ReturnFalse(string? uri)
    {
        bool result = uri.IsUriWithPortNumber();
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("http://localhost:5055")]
    [InlineData("http://localhost:5055/")]
    [InlineData("http://localhost:5055/api/path")]
    [InlineData("http://example.com:5055")]
    [InlineData("http://example.com:5055/")]
    [InlineData("http://example.com:5055/api/path")]
    public void When_IsUriWithPortNumberCalled_Given_UriWithPortNumber_Then_ReturnTrue(string? uri)
    {
        bool result = uri.IsUriWithPortNumber();
        result.ShouldBeTrue();
    }
}