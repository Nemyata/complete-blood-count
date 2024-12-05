using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Common
{
    public static class EnumExtensions
    {
        /// <summary>
        /// A generic extension method that aids in reflecting
        /// and retrieving any attribute that is applied to an <see cref="Enum"/>.
        /// </summary>
        public static TAttribute? GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            return value.GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        /// Returns Name property of <see cref="DisplayAttribute"/> applied to given enum value.
        /// </summary>
        public static string GetDisplayName(this Enum value, bool required = false)
        {
            var da = GetAttribute<DisplayAttribute>(value);
            if (da == null && required)
                throw new ArgumentException($"Enum value '{nameof(value)}' have no {nameof(DisplayAttribute)} attribute applied.", nameof(value));

            var result = da?.GetName();
            if (required && result == null)
                throw new ArgumentException($"Name of applied {nameof(DisplayAttribute)} attribute to enum value '{nameof(value)}' is null.", nameof(value));

            return result ?? value.ToString();
        }

        /// <summary>
        /// Returns GroupName property of <see cref="DisplayAttribute"/> applied to given enum value.
        /// </summary>
        public static string? GetDisplayGroupName(this Enum value, bool required = false)
        {
            var da = GetAttribute<DisplayAttribute>(value);
            if (da == null && required)
                throw new ArgumentException($"Enum value '{nameof(value)}' have no {nameof(DisplayAttribute)} attribute applied.", nameof(value));

            var result = da?.GroupName;
            if (required && result == null)
                throw new ArgumentException($"GroupName of applied {nameof(DisplayAttribute)} attribute to enum value '{nameof(value)}' is null.", nameof(value));

            return result ?? value.ToString();
        }
    }
}