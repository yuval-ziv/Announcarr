using System.Text;
using System.Text.RegularExpressions;
using Announcarr.Abstractions.Contracts;
using EnumsNET;

namespace Announcarr.Exporters.Abstractions.Exporter.Resolvers;

public static partial class TextMessageResolver
{
    [GeneratedRegex(@"(?<!\{)\{announcementType\}(?!\})")]
    private static partial Regex AnnouncementTypeRegex();

    [GeneratedRegex(@"(?<!\{)\{startDate\}(?!\})")]
    private static partial Regex StartDateTypeRegex();

    [GeneratedRegex(@"(?<!\{)\{endDate\}(?!\})")]
    private static partial Regex EndDateTypeRegex();

    [GeneratedRegex(@"\{\{")]
    private static partial Regex DoubleOpeningBracesRegex();

    [GeneratedRegex(@"\}\}")]
    private static partial Regex DoubleClosingBracesRegex();

    public static string ResolveTextMessage(string? text, AnnouncementType? announcementType = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null,
        string dateTimeFormat = "dd/MM/yyyy")
    {
        if (text is null)
            return string.Empty;

        if (announcementType is not null)
        {
            text = AnnouncementTypeRegex().Replace(text, announcementType.Value.AsString(EnumFormat.Description) ?? announcementType.Value.AsString());
        }

        if (startDate is not null)
        {
            text = StartDateTypeRegex().Replace(text, startDate.Value.ToString(dateTimeFormat));
        }

        if (endDate is not null)
        {
            text = EndDateTypeRegex().Replace(text, endDate.Value.ToString(dateTimeFormat));
        }

        return ReduceCurlyBraces(text);
    }

    private static string ReduceCurlyBraces(string input)
    {
        List<string> inputParts = DoubleOpeningBracesRegex().Split(input).ToList();
        if (inputParts.Count == 1)
        {
            return inputParts[0];
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(inputParts[0]);
        for (var currentIndex = 0; currentIndex < inputParts.Count;)
        {
            int indexOfStringWithClosingBraces = inputParts.FindIndex(currentIndex + 1, s2 => s2.Contains("}}"));

            if (indexOfStringWithClosingBraces == -1)
            {
                return stringBuilder.AppendJoin("", inputParts.Skip(currentIndex + 1)).ToString();
            }

            int amountOfClosingBraces = DoubleClosingBracesRegex().Count(inputParts[indexOfStringWithClosingBraces]);
            string stringWithClosingBraces = inputParts[indexOfStringWithClosingBraces];

            if (SameAmountOfOpeningAndClosingBraces(amountOfClosingBraces, indexOfStringWithClosingBraces, currentIndex))
            {
                stringBuilder.Append(new string('{', indexOfStringWithClosingBraces - currentIndex));
                stringBuilder.Append(stringWithClosingBraces.Replace("}}", "}"));
                currentIndex = indexOfStringWithClosingBraces;
            }

            else if (MoreOpeningBracesThanClosingBraces(amountOfClosingBraces, indexOfStringWithClosingBraces, currentIndex))
            {
                stringBuilder.Append(new string('{', amountOfClosingBraces + (indexOfStringWithClosingBraces - currentIndex - amountOfClosingBraces) * 2));
                stringBuilder.Append(stringWithClosingBraces.Replace("}}", "}"));
                currentIndex = indexOfStringWithClosingBraces;
            }

            else
            {
                stringBuilder.Append(new string('{', indexOfStringWithClosingBraces - currentIndex));
                stringBuilder.Append(DoubleClosingBracesRegex().Replace(stringWithClosingBraces, "}", indexOfStringWithClosingBraces - currentIndex));
                currentIndex = indexOfStringWithClosingBraces;
            }
        }

        return stringBuilder.ToString();
    }

    private static bool SameAmountOfOpeningAndClosingBraces(int amountOfClosingBraces, int indexOfStringWithClosingBraces, int i)
    {
        return amountOfClosingBraces == indexOfStringWithClosingBraces - i;
    }

    private static bool MoreOpeningBracesThanClosingBraces(int amountOfClosingBraces, int indexOfStringWithClosingBraces, int i)
    {
        return indexOfStringWithClosingBraces - i > amountOfClosingBraces;
    }
}