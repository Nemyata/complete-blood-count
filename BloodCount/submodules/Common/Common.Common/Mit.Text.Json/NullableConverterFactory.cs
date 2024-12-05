using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Text.Json;

/// <remarks>
/// <see href="https://stackoverflow.com/questions/65022834/how-to-deserialize-an-empty-string-to-a-null-value-for-all-nullablet-value-t">
/// c# - How to deserialize an empty string to a null value for all `Nullable&lt;T&gt;` value types using System.Text.Json? - Stack Overflow</see><br />
/// <see href="https://dotnetfiddle.net/MXnhJx">https://dotnetfiddle.net/MXnhJx</see>
/// </remarks>
public class NullableConverterFactory : JsonConverterFactory
{
    static readonly byte[] Empty = Array.Empty<byte>();

    public override bool CanConvert(Type typeToConvert)
    {
        return Nullable.GetUnderlyingType(typeToConvert) != null;
    }

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
    {
        return (JsonConverter)Activator.CreateInstance(
            typeof(NullableConverter<>).MakeGenericType(new[] { Nullable.GetUnderlyingType(type) }),
            BindingFlags.Instance | BindingFlags.Public,
            null,
            new object[] { options },
            null
        );
    }

    class NullableConverter<T> : JsonConverter<T?> where T : struct
    {
        // DO NOT CACHE the return of (JsonConverter<T>)options.GetConverter(typeof(T)) as DoubleConverter.Read() and DoubleConverter.Write()
        // DO NOT WORK for nondefault values of JsonSerializerOptions.NumberHandling which was introduced in .NET 5
        public NullableConverter(JsonSerializerOptions options) { }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                return JsonSerializer.Deserialize<T>(ref reader, options);
            if (reader.ValueTextEquals(Empty))
                return null;
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}