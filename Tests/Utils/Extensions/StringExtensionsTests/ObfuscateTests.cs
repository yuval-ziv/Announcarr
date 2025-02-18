using Announcarr.Utils.Extensions.String;
using Shouldly;

namespace Announcarr.Test.Utils.Extensions.StringExtensionsTests;

public class ObfuscateTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void When_ObfuscateCalled_Given_NullOrEmptyString_Then_ReturnEmptyString(string? input)
    {
        string result = input.Obfuscate();

        result.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("short", 3, 3)]
    [InlineData("123", 1, 3)]
    [InlineData("supercalifragilisticexpialidocious", 20, 15)]
    [InlineData("VQXIyoAhe9IcIOFRn9gkj9xjjjF08y08r0TJCgEUE4WYiSQnh3rmi9nUcOdcajKM", 32, 33)]
    public void When_ObfuscateCalled_Given_StringWithTooManyCharactersToKeep_Then_ThrowArgumentException(string input, int keepFirstCharacters, int keepLastCharacters)
    {
        Action action = () => input.Obfuscate(keepFirstCharacters: keepFirstCharacters, keepLastCharacters: keepLastCharacters);

        action.ShouldThrow<ArgumentException>()
            .Message.ShouldBe($"Input string length must be bigger than total amount of characters to skip (was {input.Length}, needed at least {keepFirstCharacters + keepLastCharacters})");
    }

    [Theory]
    [InlineData("WhatsUp")]
    [InlineData("HelloWorld")]
    [InlineData("supercalifragilisticexpialidocious")]
    public void When_ObfuscateCalled_Given_ValidString_Then_StringLengthShouldBeTheSame(string input)
    {
        string result = input.Obfuscate();

        result.Length.ShouldBe(input.Length);
    }

    [Theory]
    [InlineData("supercalifragilisticexpialidocious", 10, 5, "supercalif*******************cious")]
    [InlineData("VQXIyoAhe9IcIOFRn9gkj9xjjjF08y08r0TJCgEUE4WYiSQnh3rmi9nUcOdcajKM", 8, 8, "VQXIyoAh************************************************cOdcajKM")]
    public void When_ObfuscateCalled_Given_ValidString_Then_ResultShouldBeObfuscatedWithAsterisks(string input, int keepFirstCharacters, int keepLastCharacters, string expectedResult)
    {
        string result = input.Obfuscate(keepFirstCharacters: keepFirstCharacters, keepLastCharacters: keepLastCharacters);

        result.ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData("supercalifragilisticexpialidocious", '#', 10, 5, "supercalif###################cious")]
    [InlineData("VQXIyoAhe9IcIOFRn9gkj9xjjjF08y08r0TJCgEUE4WYiSQnh3rmi9nUcOdcajKM", 'X', 8, 8, "VQXIyoAhXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXcOdcajKM")]
    public void When_ObfuscateCalled_Given_ValidStringWithCustomObfuscationCharacter_Then_ResultShouldBeObfuscatedWithTheCustomCharacter(string input, char obfuscationCharacter,
        int keepFirstCharacters, int keepLastCharacters, string expectedResult)
    {
        string result = input.Obfuscate(obfuscationCharacter, keepFirstCharacters, keepLastCharacters);

        result.ShouldBe(expectedResult);
    }
}