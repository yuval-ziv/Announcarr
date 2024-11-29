using Announcarr.Utils.Extensions.String;
using FluentAssertions;

namespace Announcarr.Test.Utils.Extensions.StringExtensionsTests;

public class IsValidUriTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("http://")]
    [InlineData("http://localhost:65536")]
    [InlineData("http://localhost:-1")]
    [InlineData("http://localhost:abc")]
    [InlineData("http://hello,world")]
    public void When_IsValidUriCalled_Given_InvalidUri_Then_ReturnFalse(string? uri)
    {
        bool result = uri.IsValidUri();
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("localhost")]
    [InlineData("localhost:5055")]
    [InlineData("http://localhost")]
    [InlineData("http://localhost/")]
    [InlineData("http://localhost/api/path")]
    [InlineData("http://localhost:5055")]
    [InlineData("http://localhost:5055/")]
    [InlineData("http://localhost:5055/api/path")]
    [InlineData("example.com")]
    [InlineData("example.com:5055")]
    [InlineData("www.example.com")]
    [InlineData("www.example.com:5055")]
    [InlineData("http://example.com")]
    [InlineData("http://example.com/")]
    [InlineData("http://example.com/api/path")]
    [InlineData("http://example.com:5055")]
    [InlineData("http://example.com:5055/")]
    [InlineData("http://example.com:5055/api/path")]
    [InlineData("ftp://example.com")]
    [InlineData("https://example.com")]
    public void When_IsValidUriCalled_Given_ValidUri_Then_ReturnTrue(string? uri)
    {
        bool result = uri.IsValidUri();
        result.Should().BeTrue();
    }
}