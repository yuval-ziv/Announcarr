using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Announcarr.Utils.Extensions.String;

public static partial class StringExtensions
{
    [GeneratedRegex(@"[-_\s]+")]
    private static partial Regex WhiteSpaceRegex();

    [GeneratedRegex(@"[-_\s]+|(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z])")]
    private static partial Regex WhiteSpaceOrCapitalCaseRegex();

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string ToCamelCase(this string? value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        string[] words = GetWords(value);
        return words.First().ToLower() + string.Concat(words.Skip(1).Select(CapitalizeFirstLetter));
    }

    private static string[] GetWords(string value)
    {
        if (value.All(CapitalizedOrNonLetter))
        {
            return WhiteSpaceRegex().Split(value);
        }

        return WhiteSpaceOrCapitalCaseRegex().Split(value);
    }

    private static bool CapitalizedOrNonLetter(char character)
    {
        return char.IsUpper(character) || !char.IsLetter(character);
    }

    private static string CapitalizeFirstLetter(string word)
    {
        if (word.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        return char.ToUpper(word[0]) + word[1..].ToLower();
    }
}