using Announcarr.Utils.Extensions.String;
using Shouldly;

namespace Announcarr.Test.Utils.Extensions.StringExtensionsTests;

public class ToCamelCaseTests
{
    [Theory]
    [InlineData("hello world i am you")]
    [InlineData("HelloWorldIAmYou")]
    [InlineData("hello-world-i-am-you")]
    [InlineData("Hello World I Am You")]
    [InlineData("HELLO WORLD I AM YOU")]
    [InlineData("Hello_World_I_Am_You")]
    [InlineData("HelloWorld_I-Am You")]
    [InlineData("HelloWorld_I-AmYou")]
    [InlineData("HelloWorld_IAmYou")]
    [InlineData("Hello\t\t\tWorld___I----Am     You    ")]
    [InlineData("Hello\n\n\t_- World___I----Am     You    ")]
    public void When_ToCamelCaseCalled_Given_StringWithDifferentCasing_Then_ReturnCamelCasedString(string? input)
    {
        string result = input.ToCamelCase();
        result.ShouldBe("helloWorldIAmYou");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r")]
    [InlineData("\n")]
    public void When_ToCamelCaseCalled_Given_NullOrWhiteSpaceString_Then_ReturnEmptyString(string? input)
    {
        string result = input.ToCamelCase();
        result.ShouldBeEmpty();
    }
}