using Announcarr.Utils.Extensions.String;
using FluentAssertions;

namespace StringExtensionsTests;

public class IsNullOrWhiteSpaceTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r")]
    [InlineData("\n")]
    public void When_IsNullOrWhiteSpaceCalled_Given_NullOrWhiteSpaceString_Then_ReturnTrue(string? input)
    {
        bool result = input.IsNullOrWhiteSpace();
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("Hello, world!")]
    [InlineData("null")]
    [InlineData("ㅤ")] //This may look like a space, but it's actually the Hangul Filler character.
    public void When_IsNullOrWhiteSpaceCalled_Given_ANonNullOrWhiteSpaceString_Then_ReturnFalse(string? input)
    {
        bool result = input.IsNullOrWhiteSpace();
        result.Should().BeFalse();
    }
}