using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using SqlKata;

using Common.Data.SqlKata;

namespace Common.Data;

public static class SqlKataToolkit
{
    private static readonly Type[] NumberTypes = {
        typeof (int),
        typeof (long),
        typeof (decimal),
        typeof (double),
        typeof (float),
        typeof (short),
        typeof (ushort),
        typeof (ulong)
    };

    private static readonly MethodInfo AndInfoMethod = typeof(Query).GetMethod("And", BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, Type.EmptyTypes, null)!;
    private static readonly MethodInfo OrInfoMethod = typeof(Query).GetMethod("GetOr", BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, Type.EmptyTypes, null)!;
    private static readonly MethodInfo NotInfoMethod = typeof(Query).GetMethod("GetNot", BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, Type.EmptyTypes, null)!;

    #region Wheres
    #region Where
    public static Query Where2(this Query query, string column, string op, object value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.Where(column, op, result);
    }
    public static Query WhereNot2(this Query query, string column, string op, object value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.WhereNot(column, op, result);
    }

    public static Query OrWhere2(this Query query, string column, string op, object value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhere(column, op, result);
    }
    public static Query OrWhereNot2(this Query query, string column, string op, object value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhereNot(column, op, result);
    }

    public static Query Where2(this Query query, string column, object? value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.Where(column, result);
    }
    public static Query WhereNot2(this Query query, string column, object value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.WhereNot(column, result);
    }

    public static Query OrWhere2(this Query query, string column, object value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhere(column, result);
    }
    public static Query OrWhereNot2(this Query query, string column, object value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhereNot(column, result);
    }

    public static Query Where2(this Query query, IEnumerable<KeyValuePair<string, object>> values,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        var or = (bool)OrInfoMethod.Invoke(query, Array.Empty<object>())!;
        var not = (bool)NotInfoMethod.Invoke(query, Array.Empty<object>())!;

        foreach (KeyValuePair<string, object> keyValuePair in values)
        {
            if (or)
                query = query.Or();
            else
                AndInfoMethod.Invoke(query, Array.Empty<object>());
            query = query.Not(not).Where2(keyValuePair.Key, keyValuePair.Value, skipNull, skipEmpty, useTrim);
        }
        return query;
    }
    #endregion

    #region WhereLike
    public static Query WhereLike2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.WhereLike(column, result, caseSensitive, escapeCharacter);
    }
    public static Query WhereNotLike2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.WhereNotLike(column, result, caseSensitive, escapeCharacter);
    }

    public static Query OrWhereLike2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhereLike(column, result, caseSensitive, escapeCharacter);
    }
    public static Query OrWhereNotLike2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhereNotLike(column, result, caseSensitive, escapeCharacter);
    }
    #endregion

    #region WhereStarts
    public static Query WhereStarts2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.WhereStarts(column, result, caseSensitive, escapeCharacter);
    }
    public static Query WhereNotStarts2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.WhereNotStarts(column, result, caseSensitive, escapeCharacter);
    }

    public static Query OrWhereStarts2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhereStarts(column, result, caseSensitive, escapeCharacter);
    }
    public static Query OrWhereNotStarts2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhereNotStarts(column, result, caseSensitive, escapeCharacter);
    }
    #endregion

    #region WhereEnds
    public static Query WhereEnds2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.WhereEnds(column, result, caseSensitive, escapeCharacter);
    }
    public static Query WhereNotEnds2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.WhereNotEnds(column, result, caseSensitive, escapeCharacter);
    }

    public static Query OrWhereEnds2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhereEnds(column, result, caseSensitive, escapeCharacter);
    }
    public static Query OrWhereNotEnds2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhereNotEnds(column, result, caseSensitive, escapeCharacter);
    }
    #endregion

    #region WhereContains
    public static Query WhereContains2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.WhereContains(column, result, caseSensitive, escapeCharacter);
    }
    public static Query WhereNotContains2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.WhereNotContains(column, result, caseSensitive, escapeCharacter);
    }

    public static Query OrWhereContains2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhereContains(column, result, caseSensitive, escapeCharacter);
    }
    public static Query OrWhereNotContains2(this Query query, string column, object? value, bool caseSensitive = false, string? escapeCharacter = null,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.OrWhereNotContains(column, result, caseSensitive, escapeCharacter);
    }
    #endregion

    #region WhereInRaw
    public static Query WhereInRaw(this Query query, string column, string? value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetStringParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;

        var or = (bool)OrInfoMethod.Invoke(query, Array.Empty<object>())!;
        var not = (bool)NotInfoMethod.Invoke(query, Array.Empty<object>())!;

        var clause = new InRawCondition
        {
            Column = column,
            IsOr = or,
            IsNot = not,
            Value = result!
        };
        return query.AddComponent("where", clause);
    }
    public static Query WhereNotInRaw(this Query query, string column, string? value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetStringParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.Not().WhereInRaw(column, result);
    }

    public static Query OrWhereInRaw(this Query query, string column, string? value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetStringParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.Or().WhereInRaw(column, result);
    }
    public static Query OrWhereNotInRaw(this Query query, string column, string? value,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        if (!TryGetStringParam(value, out var result, skipNull, skipEmpty, useTrim))
            return query;
        return query.Or().Not().WhereInRaw(column, result);
    }
    #endregion
    #endregion

    public static Query AsInsert2(this Query query, IEnumerable<string> columns, Dictionary<string, string>? map = null, bool returnId = false)
    {
        query.AsInsert(GetColumnPropertyMapping(columns, map), returnId);
        return query;
    }

    public static Query AsUpdate2(this Query query, IEnumerable<string> columns, Dictionary<string, string>? map = null)
    {
        query.AsUpdate(GetColumnPropertyMapping(columns, map));
        return query;
    }

    /// <remarks>Based on implementation of <see cref="SqlResult.ToString()"/> method.</remarks>
    public static string ToParameterizedSql(this SqlResult query)
    {
        var deepParameters = Helper.Flatten(query.Bindings).ToList<object>();
        return Helper.ReplaceAll(query.RawSql, "?", i =>
        {
            if (i >= deepParameters.Count)
                throw new Exception($"Failed to retrieve a binding at index {i}, the total bindings count is {query.Bindings.Count}");
            return ChangeToSqlValue(deepParameters[i]);
        });

        static string? ChangeToSqlValue(object? value)
        {
            if (value == null)
                return "NULL";
            if (Helper.IsArray(value))
                return Helper.JoinArray(",", value as IEnumerable);
            if (NumberTypes.Contains(value.GetType()))
                return Convert.ToString(value, CultureInfo.InvariantCulture);

            switch (value)
            {
                case DateTimeOffset dateTimeOffset:
                    return $"'{dateTimeOffset:yyyy-MM-ddTHH:mm:ss.fffffffzzz}'";
                case DateOnly dateOnly:
                    return $"CONVERT(datetime, '{dateOnly:yyyy-MM-dd}', 101)";
                case DateTime dateTime:
                    return $"CONVERT(datetime, '{dateTime:yyyy-MM-dd HH:mm:ss.fff}', 101)";
                case bool flag:
                    return !flag ? "0" : "1";
                case Enum @enum:
                    return Convert.ChangeType(@enum, @enum.GetTypeCode()) + $" /* {@enum.GetType().Name}.{@enum} */";
                default:
                    var str = value.ToString()!.Replace("'", "''");
                    return str.StartsWith("@") ? str : $"'{str}'";
            }
        }
    }

    #region TryGetParam
    private static bool TryGetParam(object? value, out object? result,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        result = null;

        if (skipNull && value is null)                  // skip
            return false;
        if (value is not string str)
        {
            result = value;
            return true;                                // use `value` (original object)
        }

        if (TryGetStringParam(str, out var str2, false, skipEmpty, useTrim))
        {
            result = str2;
            return true;                                // use `str2` (revised string)
        }
        return false;
    }
    private static bool TryGetStringParam(string? value, out string? result,
        bool skipNull = true, bool skipEmpty = true, bool useTrim = true)
    {
        result = null;

        if (skipNull && value is null)                  // skip
            return false;

        if (useTrim && value != null)
            value = value.Trim();
        if (skipEmpty && string.IsNullOrEmpty(value))   // skip
            return false;

        result = value;
        return true;                                    // use `value` (revised string)
    }
    #endregion

    private static IEnumerable<KeyValuePair<string, object>> GetColumnPropertyMapping(IEnumerable<string> columns, IReadOnlyDictionary<string, string>? map = null)
    {
        var mapping = columns.Select(column =>
        {
            var property = map?.TryGetValue(column, out var p) == true ? p : column;
            return new KeyValuePair<string, object>(column, $"@{property}");
        });
        return mapping;
    }
}