using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Common;

public static class StringExtensions
{
    public static string? CapitalizeFirstLetter(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpper(value[0]) + value.Substring(1);
    }

    /// <summary>
    /// Splits a string into substrings by capital letters.
    /// Used to split names of methods, fields etc. given in CamelCase/lowerCamelCase notation.
    /// </summary>
    /// <param name="value">Value to process.</param>
    /// <returns>An array whose elements contain the substrings of <paramref name="value"/> which are starting by capital letters.</returns>
    /// <remarks>
    /// <see href="https://stackoverflow.com/questions/4488969/split-a-string-by-capital-letters">c# - Split a string by capital letters - Stack Overflow</see>
    /// </remarks>
    public static string[] SplitByCapitalizedLetters(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return Array.Empty<string>();

        return Regex.Split(value, @"(?<!^)(?=[A-Z])");
    }

    /// <remarks><see href="https://stackoverflow.com/questions/4135317/make-first-letter-of-a-string-upper-case-with-maximum-performance">
    /// c# - Make first letter of a string upper case (with maximum performance) - Stack Overflow</see></remarks>
    public static string? FirstLetterToUpper(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return string.Create(value.Length, value, (chars, state) =>
        {
            state.AsSpan().CopyTo(chars);
            chars[0] = char.ToUpper(chars[0]);
        });
    }

    /// <summary>
    /// Преобразование строки символов в GUID через SHA256 кодирование
    /// </summary>
    /// <param name="value">Не GUID строка</param>
    /// <returns>Уникальный идентификатор GUID</returns>
    public static Guid? ToGuid(this string value)
    {
        byte[] buf = Encoding.UTF8.GetBytes(value);
        byte[] guid = new byte[16];
        if (buf.Length < 16)
        {
            Array.Copy(buf, guid, buf.Length);
        }
        else
        {
            using SHA256 sha1 = SHA256.Create();
            byte[] hash = sha1.ComputeHash(buf);
            // Hash is 20 bytes, but we need 16. We loose some of "uniqueness", but I doubt it will be fatal
            Array.Copy(hash, guid, 16);
        }

        // Don't use Guid constructor, it tends to swap bytes. We want to preserve original string as hex dump.
        var guidS = "{" + String.Format("{0:X2}{1:X2}{2:X2}{3:X2}-{4:X2}{5:X2}-{6:X2}{7:X2}-{8:X2}{9:X2}-{10:X2}{11:X2}{12:X2}{13:X2}{14:X2}{15:X2}",
            guid[0], guid[1], guid[2], guid[3], guid[4], guid[5], guid[6], guid[7], guid[8], guid[9], guid[10], guid[11], guid[12], guid[13], guid[14], guid[15]) + "}";

        return Guid.Parse(guidS);
    }
}