using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Announcarr.Utils.Extensions.String;

public static partial class StringExtensions
{
    private const int DefaultAmountOfCharactersToKeep = 0;

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

    public static bool IsValidUri([NotNullWhen(true)] this string? value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return false;
        }

        return Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out Uri? _);
    }

    public static bool IsUriWithPortNumber([NotNullWhen(true)] this string? value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return false;
        }

        if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            return false;
        }

        return uri.IsAbsoluteUri ? uri.Authority.Contains(':') : uri.OriginalString.Contains(':');
    }

    public static string Obfuscate(this string? value, char obfuscationCharacter = '*', int keepFirstCharacters = DefaultAmountOfCharactersToKeep,
        int keepLastCharacters = DefaultAmountOfCharactersToKeep)
    {
        if (value.IsNullOrEmpty())
        {
            return string.Empty;
        }

        int totalCharactersToKeep = keepFirstCharacters + keepLastCharacters;

        if (value.Length < totalCharactersToKeep)
        {
            throw new ArgumentException($"Input string length must be bigger than total amount of characters to skip (was {value.Length}, needed at least {totalCharactersToKeep})");
        }

        string first = value[..keepFirstCharacters];
        var middle = new string(obfuscationCharacter, value.Length - totalCharactersToKeep);
        string last = value[^keepLastCharacters..];

        return first + middle + last;
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