using EnumsNET;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Announcarr.Utils.Json;

public class CaseAndHumpInsensitiveStringEnumConverter : StringEnumConverter
{
    private const bool IgnoreCase = true;

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.String)
        {
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        var enumString = reader.Value?.ToString();

        if (enumString is null)
        {
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        if (Enums.TryParse(objectType, enumString, IgnoreCase, out object? result))
        {
            return result;
        }

        return Enums.Parse(objectType, enumString.Replace("_", string.Empty), IgnoreCase);
    }
}