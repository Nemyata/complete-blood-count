using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;

using Common.Extensions.Configuration;
using Common.Extensions.DependencyInjection;

namespace Common.Data;

public static class ServiceCollectionExtensions
{
    public static void ConfigureMigratorSection<TSettings>(this IServiceCollection services, IConfiguration configuration)
        where TSettings : MigratorSettings
    {
        services.ConfigureSection<TSettings>(configuration);
        var defaultSchemaName = configuration.GetConfig<TSettings>()!.DefaultSchemaName ?? "dbo";
        services.AddSingleton<IConventionSet>(new DefaultConventionSet(defaultSchemaName, null));
    }

    public static void AddMigratorConnectionServices<TIConnectionService, TConnectionService>(this IServiceCollection services, int connectionType)
        where TIConnectionService : class
        where TConnectionService : class, IConnectionEssentialService, TIConnectionService
    {
        // setup connection services
        services.AddSingleton<TConnectionService>();
        services.AddSingleton<IConnectionEssentialService>(sp => sp.GetRequiredService<TConnectionService>());
        services.AddSingleton<TIConnectionService>(sp => sp.GetRequiredService<TConnectionService>());

        services.AddSingleton<IConnectionStringReader>(sp =>
            new ConnectionStringReader(sp.GetRequiredService<IConnectionEssentialService>(), connectionType)
        );
    }

    public static void AddMigrator<TProgram>(this IServiceCollection services)
    {
        // Add common FluentMigrator services
        services.AddFluentMigratorCore();
        services.ConfigureRunner(rb => rb.AddSqlServer() // Add SQLServer support to FluentMigrator
            // .WithGlobalConnectionString(connectionString) // Set the connection string
            .ScanIn(typeof(TProgram).Assembly).For.Migrations() // Define the assembly containing the migrations
        );

        // Enable logging to console in the FluentMigrator way
        services.AddLogging(lb => lb.AddFluentMigratorConsole());
    }
}