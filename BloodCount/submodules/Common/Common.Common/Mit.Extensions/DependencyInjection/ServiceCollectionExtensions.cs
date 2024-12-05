using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Common.Diagnostics;
using Common.Extensions.Configuration;

namespace Common.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static readonly Regex ConfigRx = new("(Configuration|SettingsOptions|Settings|Options|Parameters)$");

        #region AddService

        // Порядок методов соответствует дизассемблированной версии класса
        // Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions,
        // полученной с помощью ReSharper (Peek Definition, Alt + F12)

        public static IServiceCollection AddService(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            services = lifetime switch
            {
                ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
                ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
                ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return services;
        }
        public static IServiceCollection AddService(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            services = lifetime switch
            {
                ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationFactory),
                ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationFactory),
                ServiceLifetime.Transient => services.AddTransient(serviceType, implementationFactory),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return services;
        }
        public static IServiceCollection AddService<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementation : class, TService
        {
            services = lifetime switch
            {
                ServiceLifetime.Singleton => services.AddSingleton<TService, TImplementation>(),
                ServiceLifetime.Scoped => services.AddScoped<TService, TImplementation>(),
                ServiceLifetime.Transient => services.AddTransient<TService, TImplementation>(),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return services;
        }
        public static IServiceCollection AddService(this IServiceCollection services, Type serviceType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            services = lifetime switch
            {
                ServiceLifetime.Singleton => services.AddSingleton(serviceType),
                ServiceLifetime.Scoped => services.AddScoped(serviceType),
                ServiceLifetime.Transient => services.AddTransient(serviceType),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return services;
        }
        public static IServiceCollection AddService<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            services = lifetime switch
            {
                ServiceLifetime.Singleton => services.AddSingleton<TService>(),
                ServiceLifetime.Scoped => services.AddScoped<TService>(),
                ServiceLifetime.Transient => services.AddTransient<TService>(),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return services;
        }
        public static IServiceCollection AddService<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            services = lifetime switch
            {
                ServiceLifetime.Singleton => services.AddSingleton<TService>(implementationFactory),
                ServiceLifetime.Scoped => services.AddScoped<TService>(implementationFactory),
                ServiceLifetime.Transient => services.AddTransient<TService>(implementationFactory),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return services;
        }
        public static IServiceCollection AddService<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementation : class, TService
        {
            services = lifetime switch
            {
                ServiceLifetime.Singleton => services.AddSingleton<TService>(implementationFactory),
                ServiceLifetime.Scoped => services.AddScoped<TService>(implementationFactory),
                ServiceLifetime.Transient => services.AddTransient<TService>(implementationFactory),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return services;
        }
        public static IServiceCollection AddService(this IServiceCollection services, Type serviceType, object implementationInstance, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            services = lifetime switch
            {
                ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationInstance),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return services;
        }
        public static IServiceCollection AddService<TService>(this IServiceCollection services, TService implementationInstance, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : class
        {
            services = lifetime switch
            {
                ServiceLifetime.Singleton => services.AddSingleton<TService>(implementationInstance),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return services;
        }
        #endregion

        /// <summary>
        /// Post a <see cref="ConfigurationGuard"/> attached to given <see cref="IServiceCollection"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to attach a guard to.</param>
        /// <returns>Newly created instance of <see cref="ConfigurationGuard" /> attached to given <paramref name="services"></paramref>.</returns>
        public static ConfigurationGuard PostGuard(this IServiceCollection services)
        {
            return new ConfigurationGuard(services);
        }
        /// <summary>
        /// Registers a configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
        /// <param name="configuration">The configuration being bound.</param>
        /// <param name="key">Optional name of the configuration instance to bind against.
        /// If given value equals <c>null</c> the name will be determined from the name of <typeparamref name="TOptions"/> type.<br />
        /// Common nouns (Configuration|SettingsOptions|Options|Parameters) indicating role of <typeparamref name="TOptions"/> as
        /// a configuration instance will be removed from the end of the resulting string.<br />
        /// Default value is <c>null</c>.<br />
        /// <br />
        /// ConnectionStrings -> ConnectionStrings<br />
        /// FileStorage<i>Configuration</i> -> FileStorage<br />
        /// Ftp<i>SettingsOptions</i> -> Ftp<br />
        /// UbiApi<i>Options</i> -> UbiApi<br />
        /// Upload<i>Options</i> -> Upload<br />
        /// ActiveDirectory<i>Configuration</i> -> ActiveDirectory<br />
        /// NetworkDriveStore<i>Parameters</i> -> NetworkDriveStore<br />
        /// </param>
        /// <exception cref="ArgumentException"></exception>
        public static void ConfigureSection<TOptions>(this IServiceCollection services, IConfiguration configuration, string? key = null)
            where TOptions : class
        {
            key = GetSectionName(typeof(TOptions), key);

            services.Configure<TOptions>(configuration.GetSection(key));
            var value = configuration.GetConfig<TOptions>(key, false);
            if (value != default)
                return;

            var descriptor = ConfigurationGuard.GetDescriptor<TOptions>(key);
            if (!ConfigurationGuard.IsOnDuty(services))
            {
                var message = ConfigurationGuard.GetExceptionMessage(descriptor);
                throw new ArgumentException(message, nameof(services));
            }

            ConfigurationGuard.Collect(services, descriptor);
        }

        internal static string GetSectionName<TOptions>(string? key = null)
        {
            var sectionName = GetSectionName(typeof(TOptions), key);
            return sectionName;
        }
        internal static string GetSectionName(Type type, string? key = null)
        {
            if (!string.IsNullOrEmpty(key))
                return key;

            var typeName = type.Name;
            var sectionName = ConfigRx.Match(typeName).Success ? ConfigRx.Replace(typeName, "") : typeName;
            return sectionName;
        }
    }
}
