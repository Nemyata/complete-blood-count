using System;

namespace Common
{
    public static class TypeExtensions
    {
        public static object DefaultValue(this Type type)
        {
            var nullable = type.IsNullable(out var underlyingType);
            if (nullable)
                type = underlyingType;

            return (type.IsValueType || nullable) ? null : Activator.CreateInstance(type);
        }

        public static Type GetUnderlyingType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }
        public static bool IsNullable(this Type type, out Type underlyingType)
        {
            underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null;
        }
        /// <remarks>Taken from <see href="https://stackoverflow.com/a/2088849/3905944">
        /// c# - Expression.GreaterThan fails if one operand is nullable type, other is non-nullable - Stack Overflow</see></remarks>
        public static bool IsNullable(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}