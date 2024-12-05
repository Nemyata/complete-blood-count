using System.Text.Encodings.Web;
using System.Text.Json;

using DateOnlyTimeOnly.AspNet.Converters;

namespace Common.Text.Json;

public static class JsonToolkit
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    static JsonToolkit()
    {
#if NET6_0
        Options.Converters.Add(new DateOnlyJsonConverter());
        Options.Converters.Add(new TimeOnlyJsonConverter());
#endif

        Options.Converters.Add(new NullableConverterFactory());
    }

    public static TValue? Deserialize<TValue>(string value)
    {
        var result = JsonSerializer.Deserialize<TValue>(value, Options);
        return result;
    }
    public static string Serialize<TValue>(TValue value)
    {
        var result = JsonSerializer.Serialize(value, Options);
        return result;
    }
}