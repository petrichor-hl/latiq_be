using System.Text.Json;
using System.Text.Json.Serialization;

namespace LaTiQ.WebAPI.CustomJsonConverter;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (DateOnly.TryParseExact(dateString, DateFormat, out var date))
        {
            return date;
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat));
    }
}