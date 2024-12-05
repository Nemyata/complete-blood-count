using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Common.Threading;

namespace Common;

public static class TypeToolkit
{
    private delegate bool ParseFunction<T>(string s, NumberStyles style, IFormatProvider provider, out T result);

    private static readonly Regex utfPattern = new Regex("(?<!_x005F)_x(?!005F)([0-9A-F]{4})_", RegexOptions.Compiled);

    #region GetAttribute
    public static TAttribute? GetAttribute<TAttribute>(Type type, string propertyName)
        where TAttribute : Attribute
    {
        var pi = type.GetProperty(propertyName);
        return GetAttribute<TAttribute>(pi);
    }
    public static TAttribute? GetAttribute<TAttribute>(PropertyInfo? pi)
        where TAttribute : Attribute
    {
        if (pi == null)
            return default;

        var attribute = pi.GetCustomAttributes(false)
            .FirstOrDefault(x => x.GetType() == typeof(TAttribute));
        return (TAttribute?)attribute;
    }
    public static TAttribute? GetAttribute<TAttribute, T>(Expression<Func<T>> property)
        where TAttribute : Attribute
    {
        // GetAttribute(() => this.MyProperty);
        var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
        if (propertyInfo == null)
            throw new ArgumentException("The lambda expression 'property' should point to a valid Property");

        return GetAttribute<TAttribute>(propertyInfo);
    }
    #endregion

    #region TryGetValue
    public static bool TryGetValue<T>(object value, out T result)
    {
        using var a = new UseCulture();

        var targetType = typeof(T);
        var underlyingType = targetType.GetUnderlyingType();
        var isNullable = targetType.IsNullable(out _);

        object currentValue;
        try
        {
            currentValue = value;
        }
        catch
        {
            result = default;
            return false;
        }
        if (isNullable && (currentValue == null || (currentValue is string s && string.IsNullOrEmpty(s))))
        {
            result = default;
            return true;
        }
        if (targetType != typeof(string) && currentValue is T t)
        {
            result = t;
            return true;
        }
        if (TryGetDateTimeValue(out result, currentValue))
            return true;
        if (TryGetTimeSpanValue(out result, currentValue))
            return true;
        if (TryGetBooleanValue(out result, currentValue))
            return true;
        if (TryGetStringValue(out result, currentValue))
            return true;
        if (TryGetGuidValue(out result, currentValue))
            return true;
        if (currentValue.IsNumber())
        {
            try
            {
                result = (T)currentValue.CastTo(underlyingType);
                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }

        var strValue = currentValue.ToString();
        if (underlyingType == typeof(sbyte))
        {
            return TryGetBasicValue<T, sbyte>(strValue, sbyte.TryParse, out result);
        }
        if (underlyingType == typeof(byte))
        {
            return TryGetBasicValue<T, byte>(strValue, byte.TryParse, out result);
        }
        if (underlyingType == typeof(short))
        {
            return TryGetBasicValue<T, short>(strValue, short.TryParse, out result);
        }
        if (underlyingType == typeof(ushort))
        {
            return TryGetBasicValue<T, ushort>(strValue, ushort.TryParse, out result);
        }
        if (underlyingType == typeof(int))
        {
            return TryGetBasicValue<T, int>(strValue, int.TryParse, out result);
        }
        if (underlyingType == typeof(uint))
        {
            return TryGetBasicValue<T, uint>(strValue, uint.TryParse, out result);
        }
        if (underlyingType == typeof(long))
        {
            return TryGetBasicValue<T, long>(strValue, long.TryParse, out result);
        }
        if (underlyingType == typeof(ulong))
        {
            return TryGetBasicValue<T, ulong>(strValue, ulong.TryParse, out result);
        }
        if (underlyingType == typeof(float))
        {
            return TryGetBasicValue<T, float>(strValue, float.TryParse, out result);
        }
        if (underlyingType == typeof(double))
        {
            return TryGetBasicValue<T, double>(strValue, double.TryParse, out result);
        }
        if (underlyingType == typeof(decimal))
        {
            return TryGetBasicValue<T, decimal>(strValue, decimal.TryParse, out result);
        }
        if (underlyingType.IsEnum)
        {
            if (Enum.IsDefined(underlyingType, strValue))
            {
                result = (T)Enum.Parse(underlyingType, strValue, ignoreCase: false);
                return true;
            }
            result = default;
            return false;
        }

        try
        {
            result = (T)currentValue.CastTo(targetType);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
    public static bool TryGetValue(object value, Type type, out object result)
    {
        var mi = GetMethod(typeof(TypeToolkit), nameof(TryGetValue), new[] { typeof(object), typeof(T) });

        var parameters = new[] { value, null };
        var returnValue = mi
            .MakeGenericMethod(type)
            .Invoke(null, parameters);
        result = (bool)returnValue ? parameters[1] : default;
        return (bool)returnValue;
    }
    #endregion

    #region TryGetBasicValue
    private static bool TryGetBasicValue<T, U>(string currentValue, ParseFunction<U> parseFunction, out T value)
    {
        if (parseFunction(currentValue, NumberStyles.Any, null, out var result))
        {
            value = (T)Convert.ChangeType(result, typeof(T).GetUnderlyingType());
            return true;
        }
        value = default;
        return false;
    }
    private static bool TryGetBooleanValue<T>(out T value, object currentValue)
    {
        if (typeof(T) != typeof(bool) && typeof(T) != typeof(bool?))
        {
            value = default;
            return false;
        }
        if (currentValue is T v)
        {
            value = v;
            return true;
        }
        if (!bool.TryParse(currentValue.ToString(), out var b))
        {
            value = default;
            return false;
        }
        value = b.CastTo<T>();
        return true;
    }
    private static bool TryGetDateTimeValue<T>(out T value, object currentValue)
    {
        if (typeof(T) != typeof(DateTime) && typeof(T) != typeof(DateTime?))
        {
            value = default;
            return false;
        }
        if (currentValue is T v)
        {
            value = v;
            return true;
        }
        if (currentValue.IsNumber())
        {
            double dbl1 = Convert.ToDouble(currentValue);
            if (dbl1.IsValidOADateNumber())
            {
                value = DateTime.FromOADate(dbl1).CastTo<T>();
                return true;
            }
        }
        if (DateTime.TryParse(currentValue.ToString(), out var ts))
        {
            value = ts.CastTo<T>();
            return true;
        }
        value = default;
        return false;
    }
    private static bool TryGetStringValue<T>(out T value, object currentValue)
    {
        if (typeof(T) == typeof(string))
        {
            if (currentValue == null)
            {
                value = default;
                return true;
            }

            string s = currentValue.ToString();
            MatchCollection matches = utfPattern.Matches(s);
            if (matches.Count == 0)
            {
                value = s.CastTo<T>();
                return true;
            }
            var sb = new StringBuilder();
            int lastIndex = 0;
            foreach (Match match in matches.Cast<Match>())
            {
                string matchString = match.Value;
                int matchIndex = match.Index;
                sb.Append(s.Substring(lastIndex, matchIndex - lastIndex));
                sb.Append((char)int.Parse(match.Groups[1].Value, NumberStyles.AllowHexSpecifier));
                lastIndex = matchIndex + matchString.Length;
            }
            if (lastIndex < s.Length)
            {
                sb.Append(s.Substring(lastIndex));
            }
            value = sb.ToString().CastTo<T>();
            return true;
        }
        value = default;
        return false;
    }
    private static bool TryGetTimeSpanValue<T>(out T value, object currentValue)
    {
        if (typeof(T) != typeof(TimeSpan) && typeof(T) != typeof(TimeSpan?))
        {
            value = default;
            return false;
        }
        if (currentValue is T v)
        {
            value = v;
            return true;
        }
        if (!TimeSpan.TryParse(currentValue.ToString(), out var ts))
        {
            value = default;
            return false;
        }
        value = ts.CastTo<T>();
        return true;
    }
    private static bool TryGetGuidValue<T>(out T value, object currentValue)
    {
        if (typeof(T) != typeof(Guid) && typeof(T) != typeof(Guid?))
        {
            value = default;
            return false;
        }
        if (currentValue is T v)
        {
            value = v;
            return true;
        }
        if (Guid.TryParse(currentValue.ToString(), out var ts))
        {
            value = ts.CastTo<T>();
            return true;
        }
        value = default;
        return false;
    }

    private static bool IsValidOADateNumber(this double d)
    {
        return -657435.0 <= d && d < 2958466.0;
    }
    #endregion

    public static string ToString(object value, string format)
    {
        using var a = new UseCulture();

        var mi = value.GetType()
            .GetMethod("ToString", new[] { typeof(string) });

        if (mi == null)
            throw new ArgumentException($"Type '{value.GetType()}' does not include ToString(string format) method to format value '{value}'.");

        var result = (string)mi.Invoke(value, new object[]{ format }); ;
        return result;
    }

    #region GetMethod
    // Используется для получения конкретного метода с именем "TryGetValue" и generic параметром
    // См. https://stackoverflow.com/questions/4035719/getmethod-for-generic-method
    // См. https://stackoverflow.com/questions/46691470/type-getmethod-for-generic-method
    // См. https://stackoverflow.com/questions/269578/get-a-generic-method-without-using-getmethods

    /// <summary>
    /// Special type used to match any generic parameter type in GetMethod().
    /// </summary>
    public class T
    {
    }

    /// <summary>
    /// Search for a method by name and parameter types.
    /// Unlike <see cref="Type.GetMethod(string)"/>, does 'loose' matching on generic
    /// parameter types, and searches base interfaces.
    /// </summary>
    /// <exception cref="AmbiguousMatchException"/>
    public static MethodInfo GetMethod(this Type thisType, string name, params Type[] parameterTypes)
    {
        var flags = BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.FlattenHierarchy;
        return GetMethod(thisType, name, flags, parameterTypes);
    }

    /// <summary>
    /// Search for a method by name, parameter types, and binding flags.
    /// Unlike <see cref="Type.GetMethod(string)"/>), does 'loose' matching on generic
    /// parameter types, and searches base interfaces.
    /// </summary>
    /// <exception cref="AmbiguousMatchException"/>
    public static MethodInfo GetMethod(this Type thisType, string name, BindingFlags bindingFlags, params Type[] parameterTypes)
    {
        // Check all methods with the specified name, including in base classes
        GetMethod(out var matchingMethod, thisType, name, bindingFlags, parameterTypes);

        // If we're searching an interface, we have to manually search base interfaces
        if (matchingMethod != null || !thisType.IsInterface)
            return matchingMethod;

        foreach (Type interfaceType in thisType.GetInterfaces())
            GetMethod(out matchingMethod, interfaceType, name, bindingFlags, parameterTypes);
        return matchingMethod;
    }

    private static void GetMethod(out MethodInfo result, Type type, string name, BindingFlags bindingFlags, params Type[] parameterTypes)
    {
        result = default;

        // Check all methods with the specified name, including in base classes
        foreach (var member in type.GetMember(name, MemberTypes.Method, bindingFlags))
        {
            var method = (MethodInfo)member;
            // Check that the parameter counts and types match, 
            // with 'loose' matching on generic parameters
            var parameters = method.GetParameters();
            if (parameters.Length != parameterTypes.Length)
                continue;

            var i = 0;
            for (; i < parameters.Length; ++i)
            {
                if (!parameters[i].ParameterType.IsSimilarType(parameterTypes[i]))
                    break;
            }
            if (i != parameters.Length)
                continue;

            if (result != null)
                throw new AmbiguousMatchException("More than one matching method found!");
            result = method;
        }
    }

    /// <summary>
    /// Determines if the two types are either identical, or are both generic 
    /// parameters or generic types with generic parameters in the same
    ///  locations (generic parameters match any other generic paramter,
    /// but NOT concrete types).
    /// </summary>
    private static bool IsSimilarType(this Type thisType, Type type)
    {
        // Ignore any 'ref' types
        if (thisType.IsByRef)
            thisType = thisType.GetElementType();
        if (type.IsByRef)
            type = type.GetElementType();

        if (thisType == default)
            throw new NullReferenceException(nameof(thisType));
        if (type == default)
            throw new NullReferenceException(nameof(type));

        // Handle array types
        if (thisType.IsArray && type.IsArray)
            return thisType.GetElementType().IsSimilarType(type.GetElementType());

        // If the types are identical, or they're both generic parameters 
        // or the special 'T' type, treat as a match
        if (thisType == type
            || ((thisType.IsGenericParameter || thisType == typeof(T)) &&
                (type.IsGenericParameter || type == typeof(T))))
        {
            return true;
        }

        // Handle any generic arguments
        if (!thisType.IsGenericType || !type.IsGenericType)
            return false;

        Type[] thisArguments = thisType.GetGenericArguments();
        Type[] arguments = type.GetGenericArguments();
        if (thisArguments.Length != arguments.Length)
            return false;

        for (int i = 0; i < thisArguments.Length; ++i)
        {
            if (!thisArguments[i].IsSimilarType(arguments[i]))
                return false;
        }
        return true;

    }
    #endregion
}