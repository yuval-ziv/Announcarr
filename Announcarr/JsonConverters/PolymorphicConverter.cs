using System.Text.Json;
using System.Text.Json.Serialization;

namespace Announcarr.JsonConverters;

public class PolymorphicConverter<TBase> : JsonConverter<TBase>
{
    public override TBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserialization is not implemented.");
    }

    public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value!, value!.GetType(), options);
    }
}