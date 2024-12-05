using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Common.Data;

public static class SqlToolkit
{
    private const string NullTemplate = "NULL";

    public static string JoinKeys(this IEnumerable<int> values)
    {
        return values.ToList().ConvertAll(i => i.SqlRender()).JoinKeysRaw();
    }
    public static string JoinKeys(this IEnumerable<long> values)
    {
        return values.ToList().ConvertAll(i => i.SqlRender()).JoinKeysRaw();
    }
    public static string JoinKeys(this IEnumerable<char> values)
    {
        var items = values.Select(e => e.SqlRender());
        return string.Join(",", items);
    }
    public static string JoinKeys(this IEnumerable<string> values)
    {
        var items = values.Select(e => e.SqlRender());
        return string.Join(",", items);
    }
    private static string JoinKeysRaw(this IEnumerable<string> values)
    {
        return string.Join(",", values.ToArray());
    }

    public static string SqlRender(this int value)
    {
        return value.ToString();
    }
    public static string SqlRender(this int? value)
    {
        return value == null ? NullTemplate : ((int)value).SqlRender();
    }
    public static string SqlRender(this long value)
    {
        return value.ToString();
    }
    public static string SqlRender(this long? value)
    {
        return value == null ? NullTemplate : ((long)value).SqlRender();
    }
    public static string SqlRender(this char value)
    {
        return value.SqlQuote();
    }
    public static string SqlRender(this char? value)
    {
        return value == null ? NullTemplate : ((char)value).SqlRender();
    }
    public static string SqlRender(this string? value)
    {
        return value == null ? NullTemplate : value.Replace("'", "''").SqlQuote();
    }
    public static string SqlRender(this string? value, int length)
    {
        return value == null ? NullTemplate : value.StrLeft(length).SqlRender();
    }
    public static string SqlRender(this DateTime value)
    {
        return $"CONVERT(datetime, '{value:yyyy-MM-dd HH:mm:ss.fff}', 101)";
    }
    public static string SqlRender(this DateTime? value)
    {
        return value == null ? NullTemplate : ((DateTime)value).SqlRender();
    }
    public static string SqlRender(this bool value)
    {
        return value ? "1" : "0";
    }
    public static string SqlRender(this bool? value)
    {
        return value == null ? NullTemplate : ((bool)value).SqlRender();
    }
    public static string SqlRender(this decimal value)
    {
        return value.SqlRender(19, 4);
    }
    public static string SqlRender(this decimal value, int precision, int scale)
    {
        if (precision < 1)
            throw new ArgumentException("Precision must be greater than zero.");
        if (scale < 1)
            throw new ArgumentException("Scale must be greater than zero.");
        if (precision < scale)
            throw new ArgumentException("Precision must be greater than scale.");
        if (precision > 38)
            throw new ArgumentException("Precision must be less than 39.");

        if (value > (long)Math.Pow(10, precision - scale))
            throw new ArgumentException($"Value {value.ToString(CultureInfo.InvariantCulture)} is too great. Conversion to numeric({precision}, {scale}) cannot be made without a risk of losing information.");

        var result = decimal.Round(value, scale, MidpointRounding.AwayFromZero);
        if (result != value)
            throw new ArgumentException($"Conversion to numeric({precision}, {scale}) cannot be made without a risk of losing information.");

        return result.ToString("G29", CultureInfo.InvariantCulture);
    }
    public static string SqlRender(this decimal? value, int precision, int scale)
    {
        return value == null ? NullTemplate : ((decimal)value).SqlRender(precision, scale);
    }

    public static string? StrLeft(this string? value, int length)
    {
        return StrLeft(value, length, false);
    }
    public static string? StrLeft(this string? value, int length, bool ellipsis)
    {
        if (value == null || value.Length <= length)
            return value;
        return ellipsis ? value.Substring(0, length) + "…" : value.Substring(0, length);
    }
    public static string? StrRight(this string? value, int length)
    {
        return value == null || value.Length <= length ? value : value.Substring(value.Length - length, length);
    }

    private static string SqlQuote(this object value)
    {
        return $"'{value}'";
    }
}