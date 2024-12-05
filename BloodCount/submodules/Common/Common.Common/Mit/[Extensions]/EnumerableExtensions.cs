using System;
using System.Collections.Generic;

namespace Common;

public static class EnumerableExtensions
{
    /// <summary>Retrieves the value of the current <see cref="System.Collections.Generic.IEnumerable{T}" /> object,
    /// or the empty array of <typeparamref name="T"/>.</summary>
    /// <returns>The original value of the <paramref name="value"/> if the it's not equal to <see langword="null" />;
    /// otherwise, the the empty array of <typeparamref name="T"/>.</returns>
    public static IEnumerable<T> GetValueOrEmpty<T>(this IEnumerable<T>? value)
    {
        return value ?? Array.Empty<T>();
    }
}
