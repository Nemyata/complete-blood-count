using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Linq;

public static class QueryableExtensions
{
    /// <summary>Bypasses a number of elements specified by <paramref name="skip"/> in a sequence
    /// and then returns a number of contiguous elements specified by <paramref name="take"/>.</summary>
    /// <param name="source">An <see cref="IQueryable&lt;TSource&gt;" /> to return elements from.</param>
    /// <param name="skip">The number of elements to skip before returning the remaining elements.</param>
    /// <param name="take">The number of elements to return.</param>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" /> is <see langword="null" />.</exception>
    /// <returns>An <see cref="IQueryable&lt;TSource&gt;" /> that contains the specified number of elements
    /// that occur after the specified index in <paramref name="source" />.</returns>
    public static IQueryable<TSource> Frame<TSource>(this IQueryable<TSource> source, int? skip, int? take)
    {
        if (skip != null)
            source = source.Skip((int)skip);
        if (take != null)
            source = source.Take((int)take);

        return source;
    }

    /// <summary>Loads a <see cref="List&lt;TSource&gt;" /> from an <see cref="IQueryable&lt;TSource&gt;" />
    /// using paging with given options.</summary>
    /// <param name="source">The <see cref="IQueryable&lt;TSource&gt;" /> to create a <see cref="T:System.Collections.Generic.List`1" /> from.</param>
    /// <param name="skip">The number of elements to skip before returning the remaining elements.</param>
    /// <param name="take">The number of elements to return on each page loading.</param>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" /> is <see langword="null" />.</exception>
    /// <returns>A <see cref="List&lt;TSource&gt;" /> that contains elements from the input sequence.</returns>
    public static List<TSource> ByFrame<TSource>(this IQueryable<TSource> source, int? skip, int take)
    {
        var result = new List<TSource>();
        var pos = skip ?? 0;
        do
        {
            var chunk = source.Frame(pos, take).ToList();
            result.AddRange(chunk);

            if (chunk.Count < take)
                break;
            pos += take;
        }
        while (true);

        return result;
    }

    /// <summary>Sorts the elements of a sequence in order specified by <paramref name="sortOrder"/> according to a key.</summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="sortOrder">Order direction.</param>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by the function that is represented by <paramref name="keySelector" />.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
    /// <returns>An <see cref="IQueryable&lt;TSource&gt;" /> whose elements are sorted in order specified by <paramref name="sortOrder"/> according to a key.</returns>
    /// <remarks><see href="https://stackoverflow.com/questions/31661781/pass-orderby-or-orderbydescending-as-parameter">
    /// c# - Pass orderBy or OrderByDescending as parameter - Stack Overflow</see></remarks>
    public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector,
        ListSortDirection sortOrder)
    {
        return sortOrder == ListSortDirection.Ascending
            ? source.OrderBy(keySelector)
            : source.OrderByDescending(keySelector);
    }

    /// <summary>Sorts the elements of a sequence in order specified by <paramref name="sortOrder"/> according to a key.</summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="propertyName">Name of property or field containing the key of an element.</param>
    /// <param name="sortOrder">Order direction.</param>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="NullReferenceException">
    /// <paramref name="propertyName" /> is not refer to a property or a field of <typeparamref name="TSource"/>.</exception>
    /// <returns>An <see cref="IQueryable&lt;TSource&gt;" /> whose elements are sorted in order specified by <paramref name="sortOrder"/> according to a key.</returns>
    /// <remarks><see href="https://stackoverflow.com/questions/7265186/how-do-i-specify-the-linq-orderby-argument-dynamically">
    /// c# - How do I specify the Linq OrderBy argument dynamically? - Stack Overflow</see></remarks>
    public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source,
        string propertyName,
        ListSortDirection sortOrder)
    {
        var sourceType = typeof(TSource);
        var propertyInfo = sourceType.GetProperty(propertyName);
        if (propertyInfo == null)
            throw new ArgumentException($"Cannot get {nameof(PropertyInfo)} from given {nameof(propertyName)}.");

        var parameterExp = Expression.Parameter(sourceType, "type");

        var resultExpression = Expression.Call(
            typeof(Queryable),                                                          // class:               Queryable                    Queryable
            sortOrder == ListSortDirection.Ascending ? "OrderBy" : "OrderByDescending", // class's method:      OrderBy | OrderByDescending  Queryable.OrderBy
            new[] { sourceType, propertyInfo.PropertyType },                            // method's type args:  <TSource, TKey>              Queryable<TSource, TKey>.OrderBy
            source.Expression,                                                          // method's arg1:       source                       Queryable<TSource, TKey>.OrderBy(source.Expression
            Expression.Quote(Expression.Lambda(
                Expression.MakeMemberAccess(parameterExp, propertyInfo),
                parameterExp
            ))                                                                          // method's arg2:       keySelector                  Queryable<TSource, TKey>.OrderBy(source.Expression, Expression.Quote(…))
        );
        return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(resultExpression);
    }
}
