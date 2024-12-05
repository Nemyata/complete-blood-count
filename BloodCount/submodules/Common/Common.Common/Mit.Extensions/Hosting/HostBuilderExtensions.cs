using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Extensions.Hosting;

/// <summary>
/// Extensions to emulate a typical "Startup.cs" pattern for <see cref="IHostBuilder"/>
/// </summary>
/// 
/// <remarks>See <see href="https://stackoverflow.com/questions/41407221/startup-cs-in-a-self-hosted-net-core-console-application">
/// c# - Startup.cs in a self-hosted .NET Core Console Application - Stack Overflow</see> <br />
/// and an example of it here: <see href="https://github.com/sonicmouse/Host.CreateDefaultBuilder.Example"/>.</remarks>
public static class HostBuilderExtensions
{
    private const string ConfigureServicesMethodName = "ConfigureServices";

    /// <summary>
    /// Specify the startup type to be used by the host.
    /// </summary>
    /// <typeparam name="TStartup">The type containing an optional constructor with
    /// an <see cref="IConfiguration"/> parameter. The implementation should contain a public
    /// method named ConfigureServices with <see cref="IServiceCollection"/> parameter.</typeparam>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to initialize with TStartup.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public static IHostBuilder UseStartup<TStartup>(this IHostBuilder hostBuilder)
        where TStartup : class
    {
        // Invoke the ConfigureServices method on IHostBuilder...
        hostBuilder.ConfigureServices((ctx, serviceCollection) =>
        {
            var startupType = typeof(TStartup);

            // Find a method that has this signature: ConfigureServices(IServiceCollection)
            var cfgServicesMethod = startupType.GetMethod(ConfigureServicesMethodName, new[] { typeof(IServiceCollection) });

            // Check if TStartup has a ctor that takes a IConfiguration parameter
            var hasConfigCtor = startupType.GetConstructor(new[] { typeof(IConfiguration) }) != null;

            // create a TStartup instance based on ctor
            var startUpObj = hasConfigCtor
                ? (TStartup)Activator.CreateInstance(startupType, ctx.Configuration)!
                : (TStartup)Activator.CreateInstance(startupType, null)!;

            // finally, call the ConfigureServices implemented by the TStartup object
            cfgServicesMethod?.Invoke(startUpObj, new object[] { serviceCollection });
        });

        // chain the response
        return hostBuilder;
    }
}