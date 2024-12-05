using Microsoft.Extensions.Hosting;
using Serilog;

namespace Common.Diagnostics;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseSerilogCommonConfiguration<TProgram>(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()//Для отладки
                .Enrich.WithProperty("ApplicationName", typeof(TProgram).Assembly.GetName().Name)
                .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment)
                .Enrich.WithProperty("Version", typeof(TProgram).Assembly.GetName().Version);
        });

        return hostBuilder;
    }
}