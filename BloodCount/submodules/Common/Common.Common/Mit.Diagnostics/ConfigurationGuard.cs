using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Diagnostics
{
    public class ConfigurationGuard : IDisposable
    {
        private static readonly Dictionary<IServiceCollection, List<string>> MissingConfigurationSections = new();

        private readonly IServiceCollection services;

        public ConfigurationGuard(IServiceCollection services)
        {
            this.services = services;
            if (!MissingConfigurationSections.ContainsKey(services))
                MissingConfigurationSections.Add(services, new List<string>());
        }
        ~ConfigurationGuard()
        {
            Dispose(disposing: false);
        }

        internal static bool IsOnDuty(IServiceCollection services)
        {
            return MissingConfigurationSections.ContainsKey(services);
        }
        internal static void Collect(IServiceCollection services, string descriptor)
        {
            if (!IsOnDuty(services))
                return;

            var list = MissingConfigurationSections[services];
            if (list.Contains(descriptor))
                return;

            list.Add(descriptor);
        }

        internal static string GetDescriptor<TOptions>(string sectionName)
        {
            var descriptor = $"'{sectionName}' ({typeof(TOptions).FullName})";
            return descriptor;
        }
        internal static string GetExceptionMessage(string descriptor)
        {
            var message = GetExceptionMessage(new[] { descriptor });
            return message;
        }
        internal static string GetExceptionMessage(IEnumerable<string> descriptors)
        {
            var items = descriptors.ToArray();
            var plural = items.Length > 1;
            var descriptorEnumeration = string.Join(", ", items);

            var message = $"Configuration section{(plural ? "s" : string.Empty)}: {descriptorEnumeration} {(plural ? "have" : "has")} not been found.";
            return message;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (!MissingConfigurationSections.ContainsKey(services))
                return;

            var missingSections = MissingConfigurationSections[services].ToArray();
            MissingConfigurationSections.Remove(services);

            if (missingSections.Length < 1)
                return;

            var message = GetExceptionMessage(missingSections);
            throw new ArgumentException(message, nameof(services));
        }
    }
}
