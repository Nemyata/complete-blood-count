using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Common.Threading;

namespace Common
{
    /// <summary>
    /// <see cref="http://madreflection.originalcoder.com/2009/12/generic-tryparse.html"/>.</summary>
    public static class GenericTryParse
    {
        public delegate bool TryParseDelegate<T>(string s, out T result);

        private static readonly object SyncObject = new();
        private static readonly Dictionary<Type, MethodInfo> TryParseMethods = new();

        static GenericTryParse() {
            SetTryParseMethod<string>(TryParseString);
        }

        /// <summary>
        /// Converts the string representation of
        /// the specified type to its equivalent of the specified type.
        /// A return value indicates whether the conversion succeeded.</summary>
        /// <param name="s">A string containing the value to convert.</param>
        /// <param name="type">The type to which the string is to be converted.</param>
        /// <param name="result">When this method returns,
        /// contains the value converted from <paramref name="s"/> if
        /// the conversion succeeded, or the default declared value for
        /// the type if the conversion failed.</param>
        /// <returns>true if <paramref name="s"/> was converted successfully;
        /// otherwise, false.</returns>
        public static bool TryParse(this string s, Type type, out object result) {
            return TryParse(s, type, true, out result);
        }
        /// <summary>
        /// Converts the string representation of
        /// the specified type to its equivalent of the specified type.
        /// A return value indicates whether the conversion succeeded.</summary>
        /// <param name="s">A string containing the value to convert.</param>
        /// <param name="type">The type to which the string is to be converted.</param>
        /// <param name="throwOnError">Specifying whether an error will be thrown
        /// when the appropriate parse method did not found.</param>
        /// <param name="result">When this method returns,
        /// contains the value converted from <paramref name="s"/> if
        /// the conversion succeeded, or the default declared value for
        /// the type if the conversion failed.</param>
        /// <returns>true if <paramref name="s"/> was converted successfully;
        /// otherwise, false.</returns>
        public static bool TryParse(this string s, Type type, bool throwOnError, out object result) {
            result = null;

            var method = GetTryParseMethod(type);
            if (method == null) {
                if (throwOnError)
                    throw new Exception($"No suitable TryParse method found for type '{type.FullName}'.");
                return false;
            }
            var parameters = new object[] { s, null };
            var success = (bool)method.Invoke(null, parameters);
            if (success)
                result = parameters[1];
            return success;
        }
        /// <summary>
        /// Converts the string representation of
        /// type <typeparamref name="T"/> to its equivalent of
        /// type <typeparamref name="T"/>.
        /// A return value indicates whether the conversion succeeded.</summary>
        /// <typeparam name="T">The type of the value to convert.</typeparam>
        /// <param name="s">A string containing the value to convert.</param>
        /// <param name="result">When this method returns,
        /// contains the value converted from <paramref name="s"/> if
        /// the conversion succeeded, or the default declared value for
        /// the type if the conversion failed.</param>
        /// <returns>true if <paramref name="s"/> was
        /// converted successfully; otherwise, false.</returns>
        public static bool TryParse<T>(this string s, out T result) {
            return TryParse(s, true, out result);
        }
        /// <summary>
        /// Converts the string representation of
        /// type <typeparamref name="T"/> to its equivalent of
        /// type <typeparamref name="T"/>.
        /// A return value indicates whether the conversion succeeded.</summary>
        /// <typeparam name="T">The type of the value to convert.</typeparam>
        /// <param name="s">A string containing the value to convert.</param>
        /// <param name="throwOnError">Specifying whether an error will be thrown
        /// when the appropriate parse method did not found.</param>
        /// <param name="result">When this method returns,
        /// contains the value converted from <paramref name="s"/> if
        /// the conversion succeeded, or the default declared value for
        /// the type if the conversion failed.</param>
        /// <returns>true if <paramref name="s"/> was
        /// converted successfully; otherwise, false.</returns>
        public static bool TryParse<T>(this string s, bool throwOnError, out T result) {
            result = default;
            var success = TryParse(s, typeof(T), throwOnError, out var tempResult);
            if (success)
                result = (T)tempResult;
            return success;
        }

        public static T? TryParse<T>(this string s)
            where T : struct {
            return s.TryParse(out T result) ? result : null;
        }
        public static T? TryInvariantParse<T>(this string s)
            where T : struct {
            using (new UseCulture())
                return s.TryParse<T>();
        }

        /// <summary>
        /// Supplies a method to parse
        /// type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The type that
        /// the method is able to convert.</typeparam>
        /// <param name="method">A method that can
        /// parse type <typeparamref name="T"/>.</param>
        private static void SetTryParseMethod<T>(MethodInfo method) {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            var type = typeof(T);

            if (GetTryParseMethod(type) != null)
                throw new Exception($"The type '{type.FullName}' has a TryParse method available. Either the type defines one or one has already been explicitly provided.");

            if (!HasTryParseSignature(method, type))
                throw new Exception("The provided method does not match the required signature.");

            TryParseMethods[type] = method;
        }
        /// <summary>
        /// Supplies a method to parse
        /// type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The type that
        /// the method is able to convert.</typeparam>
        /// <param name="tryParseDelegate">A method that can
        /// parse type <typeparamref name="T"/>.</param>
        private static void SetTryParseMethod<T>(TryParseDelegate<T> tryParseDelegate) {
            if (tryParseDelegate == null)
                throw new ArgumentNullException(nameof(tryParseDelegate));

            SetTryParseMethod<T>(tryParseDelegate.Method);
        }

        private static bool TryParseString(string s, out string result) {
            result = s;
            return true;
        }

        public static decimal? TryParseDecimal(this string s) {
            return decimal.TryParse(s.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : null;
        }
        public static decimal ParseDecimal(this string s) {
            var result = s.TryParseDecimal();
            if (result == null)
                throw new ApplicationException();
            return (decimal)result;
        }

        private static MethodInfo GetTryParseMethod(Type type) {
            if (TryParseMethods.ContainsKey(type))
                return TryParseMethods[type];

            lock (SyncObject) {
                if (!TryParseMethods.ContainsKey(type))
                    TryParseMethods.Add(type, FindTryParseMethod(type));
            }

            return TryParseMethods[type];
        }
        private static MethodInfo FindTryParseMethod(Type type) {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;
            var parameterTypes = new[] { typeof(string), type.MakeByRefType() };

            var method = type.GetMethod("TryParse", bindingFlags, null, parameterTypes, null);
            if (method == null)
                return null;

            return HasTryParseSignature(method, type) ? method : null;
        }

        private static bool HasTryParseSignature(MethodInfo method, Type type) {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (method.ContainsGenericParameters || !method.IsStatic || method.ReturnType != typeof(bool))
                return false;

            var parameters = method.GetParameters();

            return parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == type.MakeByRefType();
        }
    }
}