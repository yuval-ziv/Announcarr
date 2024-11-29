using Announcarr.Utils.Extensions.String;
using FluentAssertions;

namespace Announcarr.Test.Utils.Extensions.StringExtensionsTests;

public class IsNullOrEmptyTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void When_IsNullOrEmptyCalled_Given_NullOrEmptyString_Then_ReturnTrue(string? input)
    {
        bool result = input.IsNullOrEmpty();
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("Hello, world!")]
    [InlineData("null")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r")]
    public void When_IsNullOrEmptyCalled_Given_ANonNullOrEmptyString_Then_ReturnFalse(string? input)
    {
        bool result = input.IsNullOrEmpty();
        result.Should().BeFalse();
    }
}