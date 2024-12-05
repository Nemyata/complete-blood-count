using Common.Data;
using Common.Data.Hosting;

using Microsoft.Extensions.Options;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;


namespace BloodCount.Migrator.Services;

internal class MigratorAppService(IHostApplicationLifetime applicationLifetime,
                                  IServiceProvider serviceProvider,
                                  IConnectionEssentialService connectionService,
                                  IOptions<MigratorSettings> settings) : MigratorEssentialAppService(applicationLifetime, serviceProvider, connectionService, settings)
{
    public override void Run(IServiceProvider serviceProvider, IMigrationRunner runner)
    {
        var connectionReader = serviceProvider.GetRequiredService<IConnectionStringAccessor>();
        InitiateDatabase(connectionReader.ConnectionString);
        MigrateUp(runner);

        //Rollback(runner, 1);
        // RollbackMigration<_000000000000_Migration>(serviceProvider, connectionReader.ConnectionString);
    }
}