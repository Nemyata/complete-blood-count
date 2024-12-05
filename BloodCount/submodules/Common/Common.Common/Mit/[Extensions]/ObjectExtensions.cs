using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;

namespace Common;

public static class ObjectExtensions
{
    public static T CastTo<T>(this object o)
    {
        return (T)o.CastTo(typeof(T));
    }
    //public static object CastTo(this object o, Type type)
    //{
    //    return CastTo(o, type);

    //    var conv = TypeDescriptor.GetConverter(type);
    //    return (T)conv.ConvertFrom(o);
    //    return (T)Convert.ChangeType(o, type);
    //}
    public static object CastTo(this object o, Type type)
    {
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
            return Convert.ChangeType(o, type);
        if (o == null)
            return null;
        type = Nullable.GetUnderlyingType(type) ?? type;

        return Convert.ChangeType(o, type);
    }

    public static bool TryCastTo<T>(this object o, out T value)
    {
        var result = TryCastTo(o, typeof(T), out var value2);
        value = (T)value2;
        return result;
    }
    public static bool TryCastTo(this object o, Type type, out object value)
    {
        value = default;

        if (type == null
            || o == null
            || !(o is IConvertible) || !(o.GetType() == type))
            return false;

        try
        {
            value = CastTo(o, type);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsNumber(this object value)
    {
        if (value is not sbyte
            && value is not byte
            && value is not short
            && value is not ushort
            && value is not int
            && value is not uint
            && value is not long
            && value is not ulong
            && value is not float
            && value is not double)
        {
            return value is decimal;
        }
        return true;
    }

    /// <summary>Sets the property value of a specified <see cref="ExpandoObject"/> instance.</summary>
    /// <param name="obj">The object whose property value will be set.</param>
    /// <param name="propertyName">The string containing the name of the public property to set.</param>
    /// <param name="value">The new property value.</param>
    /// <example>
    /// x.Set("Name", "Bill");
    /// x.LastName = "Gates";
    /// </example>
    /// 
    /// <remarks>
    /// Основано на <see href="https://stackoverflow.com/questions/7478048">c# - Why can't I do this: dynamic x = new ExpandoObject { Foo = 12, Bar = "twelve" } - Stack Overflow</see>.
    /// </remarks>
    public static void Set(ExpandoObject obj, string propertyName, object value)
    {
        IDictionary<string, object> dic = obj;
        dic[propertyName] = value;
    }

    /// <summary>Creates an <see cref="ExpandoObject"/> instance from dynamic object.</summary>
    /// <param name="source">A dynamic object to create an <see cref="ExpandoObject"/> from.</param>
    /// <returns>An <see cref="ExpandoObject"/> instance that contains the fields from the input dynamic object.</returns>
    /// <remarks>
    /// Основано на <see href="https://stackoverflow.com/questions/7478048">c# - Why can't I do this: dynamic x = new ExpandoObject { Foo = 12, Bar = "twelve" } - Stack Overflow</see>.
    /// </remarks>
    public static ExpandoObject ToExpando(this object source)
    {
        var obj = new ExpandoObject();
        IDictionary<string, object> dic = obj!;

        var type = source.GetType();
        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            dic.Add(prop.Name, prop.GetValue(source));
        }
        return obj;
    }

    /// <remarks>
    /// Based on <see href="https://stackoverflow.com/questions/10454519/best-way-to-compare-two-complex-objects#36125258">
    /// c# - Best way to compare two complex objects - Stack Overflow#36125258</see></remarks>
    public static bool Compare(this object? obj, object? another, bool json = false, bool deep = false)
    {
        if (ReferenceEquals(obj, another))
            return true;
        if (obj == null || another == null)
            return false;
        if (obj.GetType() != another.GetType())
            return false;
        if (!obj.GetType().IsClass)
            return obj.Equals(another);

        if (json)
        {
            var objJson = JsonSerializer.Serialize(obj);
            var anotherJson = JsonSerializer.Serialize(another);

            return objJson == anotherJson;
        }

        foreach (var property in obj.GetType().GetProperties())
        {
            var objValue = property.GetValue(obj);
            var anotherValue = property.GetValue(another);

            if (deep)
            {
                if (!objValue.Compare(anotherValue, json, deep))
                    return false;
            }
            else
            {
                if (!Equals(objValue, anotherValue))
                    return false;
            }
        }

        return true;
    }
}
