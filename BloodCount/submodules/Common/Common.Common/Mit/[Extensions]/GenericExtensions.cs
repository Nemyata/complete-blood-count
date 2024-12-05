using System.Collections.Generic;

namespace Common;

public static class GenericExtensions
{
    /// <summary>Determines whether the specified object instances are considered equal.</summary>
    /// <typeparam name="T">The base object type.</typeparam>
    /// <param name="a">The first object to compare.</param>
    /// <param name="b">The second object to compare.</param>
    /// <returns>
    /// <see langword="true" /> if the objects are considered equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>
    /// <see href="https://stackoverflow.com/questions/390900/cant-operator-be-applied-to-generic-types-in-c">
    /// Can't operator == be applied to generic types in C#? - Stack Overflow</see><br />
    /// <see href="https://stackoverflow.com/questions/488250/c-sharp-compare-two-generic-values">
    /// c# compare two generic values - Stack Overflow</see></remarks>
    public static bool EqualTo<T>(this T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }
}
