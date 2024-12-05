using System;
using System.Globalization;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

using Common.Data;
using Common.Threading;

namespace Common;

internal static class TypeToolkitExt
{
    private static readonly Regex decimalComma = new Regex(@"\,", RegexOptions.Compiled);

    #region TryGetValue
    public static bool TryGetValue(IXLCell cell, Type type, out object? result)
    {
        var mi = TypeToolkit.GetMethod(typeof(IXLCell), nameof(IXLCell.TryGetValue), new[] { typeof(TypeToolkit.T) });

        var parameters = new object[1];
        var returnValue = mi
            .MakeGenericMethod(type)
            .Invoke(cell, parameters);
        result = (bool)returnValue! ? parameters[0] : default;
        return (bool)returnValue;
    }
    #endregion

    #region TryGetValue
    public static bool TryGetValue<T>(string? value, DataField<T> field, out object? result)
    {
        using var _ = new UseCulture();

        var type = field.PropertyType;
        if (type.IsNullable(out var underlyingType))
            type = underlyingType;

        if (string.IsNullOrEmpty(value))
        {
            result = field.DefaultValue ?? type.DefaultValue();
            return true;
        }

        if (value.TryParse(type, out result))
            return true;
        if (!string.IsNullOrEmpty(field.Format)
            && type == typeof(DateTime)
            && DateTime.TryParseExact(value, field.Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            result = dt;
            return true;
        }
        if (!string.IsNullOrEmpty(field.Format)
            && type == typeof(Guid)
            && Guid.TryParseExact(value, field.Format, out var guid))
        {
            result = guid;
            return true;
        }

        result = type.DefaultValue();
        return false;
    }
    #endregion

    internal static object? NormalizeValue(object? value, Type type)
    {
        if (value == default)
            return value;
        if (value is string str && (type == typeof(float) || type == typeof(double) || type == typeof(decimal)))
            return decimalComma.Replace(str, ".");
        return value;
    }
}