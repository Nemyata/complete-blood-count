using BloodCount.Migrator.Services;
using BloodCount.Domain.Configuration;
using BloodCount.DataAccess.Interfaces;
using BloodCount.DataAccess.Implementation;

using Common.Data;
using Common.Diagnostics;
using Common.Extensions.Hosting;
using Common.Extensions.DependencyInjection;

using Serilog;



Log.Logger = SerilogToolkit.CreateBootstrapCommonLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddSingleton(configuration);

        services.ConfigureMigratorSection<MigratorSettings>(configuration);
        services.ConfigureSection<ConnectionStrings>(configuration);
        services.AddMigratorConnectionServices<IConnectionService, ConnectionService>((int)ConnectionType.Main);
        services.AddMigrator<Program>();

        services.AddHostedService<MigratorAppService>();
    })
    .UseSerilogCommonConfiguration<Program>()
    .Build();

return await host.TryRunAsync();