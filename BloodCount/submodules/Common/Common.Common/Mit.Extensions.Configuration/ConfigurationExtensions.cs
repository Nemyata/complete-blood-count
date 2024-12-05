using System;
using Microsoft.Extensions.Configuration;

using Common.Extensions.DependencyInjection;

namespace Common.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Attempts to bind the configuration instance with the key based on name of <typeparamref name="TOptions"/>
        /// to a new instance of type <typeparamref name="TOptions"/>.
        /// If this configuration section has a value, that will be used.
        /// Otherwise binding by matching property names against configuration keys recursively.</summary>
        /// <typeparam name="TOptions">The type of the new instance to bind.</typeparam>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="key">The key of the configuration section.</param>
        /// <param name="required">If <typeparamref name="TOptions"/> is missing, an <see cref="InvalidOperationException"/> will be thrown. <br />
        /// Default value is <c>null</c>.</param>
        /// <returns>The new instance of <typeparamref name="TOptions"/> if successful, <c>default(</c><typeparamref name="TOptions"/><c>)</c> otherwise.</returns>
        public static TOptions? GetConfig<TOptions>(this IConfiguration configuration, string? key = null, bool required = true)
            where TOptions : class
        {
            var sectionName = ServiceCollectionExtensions.GetSectionName<TOptions>(key);
            var section = configuration.GetSection(sectionName).Get<TOptions>();

            if (required && section == null)
                throw new InvalidOperationException($"Configuration section '{sectionName}' cannot be found.");

            return section;
        }
    }
}
