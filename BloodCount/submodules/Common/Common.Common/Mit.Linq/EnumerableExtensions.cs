using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Common.Linq.Expressions;
using MemberInfoExtensions = Common.Reflection.MemberInfoExtensions;

namespace Common.Linq;

public static class EnumerableExtensions
{
    /// <summary>
    /// Метод, возвращающий кортеж, включающий, кроме самого элемента, его индекс внутри переданной коллекции.
    /// </summary>
    /// <typeparam name="T">Тип элемента коллекции <param name="source"></param>.</typeparam>
    /// <param name="source">Коллекция для преобразования в новую коллекцию кортежей.</param>
    /// <returns>Коллекция кортежей, включающих, кроме самого элемента, его индекс в коллекции.</returns>
    /// <remarks>
    /// <see href="https://thomaslevesque.com/2019/11/18/using-foreach-with-index-in-c/">Using foreach with index in C# - Thomas Levesque's .NET Blog</see>
    /// </remarks>
    public static IEnumerable<(T item, int index)> Indexed<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }

    #region Wheres
    #region WhereContains
    public static IEnumerable<TSource> WhereContains<TSource>(this IEnumerable<TSource> source,
        Expression<Func<TSource, string?>> sourcePropertyExpression,
        string? value,
        StringComparison comparison = StringComparison.CurrentCultureIgnoreCase,
        bool skipNull = true,
        bool skipEmpty = true,
        bool useTrim = true)
    {
        if (skipNull && value == null)
            return source;
        if (useTrim && value != null)
            value = value.Trim();
        if (skipEmpty && string.IsNullOrEmpty(value))
            return source;
        return source.Where(GetContainsExpression(sourcePropertyExpression, value, comparison));
    }
    private static Func<TSource, bool> GetContainsExpression<TSource>(Expression<Func<TSource, string?>> sourcePropertyExpression,
        string? value, StringComparison comparison)
    {
        var member = MemberInfoExtensions.GetPropertyOrFieldInfo(sourcePropertyExpression);
        if (member == null)
            throw new NullReferenceException($"Cannot get {nameof(PropertyInfo)} from given {nameof(sourcePropertyExpression)}.");

        var parameterExp = Expression.Parameter(typeof(TSource), "type");
        var propertyExp = Expression.PropertyOrField(parameterExp, member.Name);

        var stringContainsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) })!;

        var exp = Expression.NotEqual(
            propertyExp,
            Expression.Constant(null, propertyExp.Type)
        ); // source.PropertyName != null
        exp = Expression.AndAlso(exp,
            Expression.Call(
                propertyExp,
                stringContainsMethodInfo,
                new Expression[]
                {
                    Expression.Constant(value, typeof(string)),
                    Expression.Constant(comparison, typeof(StringComparison))
                }
            ) // source.PropertyName.Contains(value, comparison)
        ); // source.PropertyName != null && source.PropertyName.Contains(value, comparison)

        return Expression.Lambda<Func<TSource, bool>>(exp, parameterExp).Compile();
    }
    #endregion

    #region WhereEquals
    public static IEnumerable<TSource> WhereEquals<TSource, T>(this IEnumerable<TSource> source,
        Expression<Func<TSource, T?>> sourcePropertyExpression,
        T? value,
        bool skipNull = true)
    {
        if (skipNull && value == null)
            return source;
        return source.Where(GetEqualsExpression(sourcePropertyExpression, value));
    }
    private static Func<TSource, bool> GetEqualsExpression<TSource, T>(Expression<Func<TSource, T?>> sourcePropertyExpression,
        T? value)
    {
        var member = MemberInfoExtensions.GetPropertyOrFieldInfo(sourcePropertyExpression);
        if (member == null)
            throw new NullReferenceException($"Cannot get {nameof(PropertyInfo)} from given {nameof(sourcePropertyExpression)}.");

        var parameterExp = Expression.Parameter(typeof(TSource), "type");
        Expression propertyExp = Expression.PropertyOrField(parameterExp, member.Name);
        Expression rightExp = Expression.Constant(value, typeof(T));

        ExpressionToolkit.ConvertToSameNullability(ref propertyExp, ref rightExp);

        var exp = Expression.Equal(
            propertyExp,
            rightExp
        ); // source.PropertyName == value

        return Expression.Lambda<Func<TSource, bool>>(exp, parameterExp).Compile();
    }

    public static IEnumerable<TSource> WhereEquals<TSource>(this IEnumerable<TSource> source,
        Expression<Func<TSource, string?>> sourcePropertyExpression,
        string? value,
        StringComparison comparison = StringComparison.CurrentCultureIgnoreCase,
        bool skipNull = true,
        bool skipEmpty = true,
        bool useTrim = true)
    {
        if (skipNull && value == null)
            return source;
        if (useTrim && value != null)
            value = value.Trim();
        if (skipEmpty && string.IsNullOrEmpty(value))
            return source;
        return source.Where(GetEqualsExpression(sourcePropertyExpression, value, comparison));
    }
    private static Func<TSource, bool> GetEqualsExpression<TSource>(Expression<Func<TSource, string?>> sourcePropertyExpression,
        string? value, StringComparison comparison)
    {
        var member = MemberInfoExtensions.GetPropertyOrFieldInfo(sourcePropertyExpression);
        if (member == null)
            throw new NullReferenceException($"Cannot get {nameof(PropertyInfo)} from given {nameof(sourcePropertyExpression)}.");

        var parameterExp = Expression.Parameter(typeof(TSource), "type");
        var propertyExp = Expression.PropertyOrField(parameterExp, member.Name);

        var stringEqualsMethodInfo = typeof(string).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string), typeof(string), typeof(StringComparison) })!;

        var exp = Expression.Call(
            stringEqualsMethodInfo,
            new Expression[]
            {
                propertyExp,
                Expression.Constant(value, typeof(string)),
                Expression.Constant(comparison, typeof(StringComparison))
            }
        ); // string.Equals(source.PropertyName, value, comparison)

        return Expression.Lambda<Func<TSource, bool>>(exp, parameterExp).Compile();
    }
    #endregion
    #endregion
}
