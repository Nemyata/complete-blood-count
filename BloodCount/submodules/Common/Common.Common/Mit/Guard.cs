using System;

namespace Common;

public static class Guard
{
    public static void CheckNotNull(object value, string? name = null, string? message = null)
    {
        if (value == null)
            throw new ArgumentNullException(name, message ?? $"Value of '{name}' argument cannot be null.");
    }
    public static void CheckNull(object value, string? name = null, string? message = null)
    {
        if (value != null)
            throw new ArgumentException(name, message ?? $"Value of '{name}' argument must be null.");
    }
    public static void CheckNullOrEmpty(string value, string? name = null, string? message = null)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException(name, message ?? $"Value of '{name}' argument cannot be null or empty string.");
    }

    public static void CheckEquals(long value, long expected, string? name = null, string? message = null)
    {
        if (value != expected)
            throw new IndexOutOfRangeException(message ?? $"Value of {value} is not valid for '{name}'. Value must be equal to {expected}.");
    }
    public static void CheckNotEquals(long value, long expected, string? name = null, string? message = null)
    {
        if (value == expected)
            throw new IndexOutOfRangeException(message ?? $"Value of {value} is not valid for '{name}'. Value should not be equal to {expected}.");
    }

    public static void CheckInRange(long value, long bottom, long top, string? name = null, string? message = null)
    {
        if (value < bottom || value > top)
            throw new IndexOutOfRangeException(message ?? $"Value of {value} is not valid for '{name}'. Value must be between {bottom} and {top}.");
    }

    public static void CheckGreaterThan(long value, long than, string? name = null, string? message = null)
    {
        if (value <= than)
            throw new ArgumentOutOfRangeException(message ?? $"Value of {value} is not valid for '{name}'. Value must be greater than {than}.");
    }
    public static void CheckNotGreaterThan(long value, long than, string? name = null, string? message = null)
    {
        if (value > than)
            throw new ArgumentOutOfRangeException(message ?? $"Value of {value} is not valid for '{name}'. Value must not be greater than {than}.");
    }

    public static void CheckLessThan(long value, long than, string? name = null, string? message = null)
    {
        if (value >= than)
            throw new ArgumentOutOfRangeException(message ?? $"Value of {value} is not valid for '{name}'. Value must be less than {than}.");
    }
    public static void CheckNotLessThan(long value, long than, string? name = null, string? message = null)
    {
        if (value < than)
            throw new ArgumentOutOfRangeException(message ?? $"Value of {value} is not valid for '{name}'. Value must not be less than {than}.");
    }

    public static void CheckTrue(bool value, string? name = null, string? message = null)
    {
        if (!value)
            throw new ArgumentException(message ?? $"Value of '{name}' argument must be true.");
    }
    public static void CheckFalse(bool value, string? name = null, string? message = null)
    {
        if (value)
            throw new ArgumentException(message ?? $"Value of '{name}' argument must be false.");
    }

    public static void CheckEven(long value, string? name = null, string? message = null)
    {
        if (value % 2 != 0)
            throw new ArgumentException(message ?? $"Value of {value} is not valid for '{name}'. Value must be even.");
    }
    public static void CheckOdd(long value, string? name = null, string? message = null)
    {
        if (value % 2 != 1)
            throw new ArgumentException(message ?? $"Value of {value} is not valid for '{name}'. Value must be odd.");
    }

    public static void ArgumentException(long dividend, long divisor, string? name = null, string? message = null)
    {
        if (dividend % divisor != 0)
            throw new ApplicationException(message ?? $"'{name}' ({dividend}) cannot be divided by {divisor}.");
    }
}