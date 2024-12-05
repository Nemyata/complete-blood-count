using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Dapper;
using FluentMigrator;
using FluentMigrator.Runner;

using Common.Extensions.Hosting;

namespace Common.Data.Hosting;

public abstract class MigratorEssentialAppService : AppHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MigratorSettings _settings;
    private readonly IConnectionEssentialService _connectionService;

    protected MigratorEssentialAppService(IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider,
        IConnectionEssentialService connectionService, IOptions<MigratorSettings> settings)
        : base(applicationLifetime)
    {
        _serviceProvider = serviceProvider;
        _connectionService = connectionService;
        _settings = settings.Value;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        // Put the database update into a scope to ensure
        // that all resources will be disposed.
        using var scope = _serviceProvider.CreateScope();

        // Instantiate the runner
        var runner = _serviceProvider.GetRequiredService<IMigrationRunner>();

        Run(_serviceProvider, runner);

        // Proper application exit
        return base.StartAsync(cancellationToken);
    }

    public abstract void Run(IServiceProvider serviceProvider, IMigrationRunner runner);

    public virtual void InitiateDatabase()
    {
        var connectionString = _connectionService.GetConnectionString();
        InitiateDatabase(connectionString);
    }
    public virtual void InitiateDatabase(int connectionType)
    {
        var connectionString = _connectionService.GetConnectionString(connectionType);
        InitiateDatabase(connectionString);
    }
    public virtual void InitiateDatabase(string connectionString)
    {
        using var connection = _connectionService.OpenConnection(connectionString);
        InitiateDatabase(connection);
    }
    public virtual void InitiateDatabase(IDbConnection connection)
    {
        try
        {
            if (!string.IsNullOrEmpty(_settings.Database))
                CreateDatabase(connection, _settings.Database);

            var schemas = _settings.Schemas.GetValueOrEmpty().ToArray();
            if (schemas.Length > 1)
                CreateSchemas(connection, schemas);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public virtual void MigrateUp(IMigrationRunner runner)
    {
        try
        {
            runner.ListMigrations();
            // Execute the migrations
            runner.MigrateUp();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public virtual void Rollback(IMigrationRunner runner, int steps = 1)
    {
        try
        {
            runner.ListMigrations();
            // Execute the migrations
            runner.Rollback(steps);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public virtual void RollbackMigration<T>(IServiceProvider serviceProvider)
        where T : IMigration
    {
        using var connection = _connectionService.OpenConnection();
        RollbackMigration(serviceProvider, typeof(T), connection);
    }
    public virtual void RollbackMigration<T>(IServiceProvider serviceProvider, int connectionType)
        where T : IMigration
    {
        using var connection = _connectionService.OpenConnection(connectionType);
        RollbackMigration(serviceProvider, typeof(T), connection);
    }
    public virtual void RollbackMigration<T>(IServiceProvider serviceProvider, string connectionString)
        where T : IMigration
    {
        using var connection = _connectionService.OpenConnection(connectionString);
        RollbackMigration(serviceProvider, typeof(T), connection);
    }
    public virtual void RollbackMigration<T>(IServiceProvider serviceProvider, IDbConnection connection)
        where T : IMigration
    {
        RollbackMigration(serviceProvider, typeof(T), connection);
    }
    public virtual void RollbackMigration(IServiceProvider serviceProvider, Type migrationType, IDbConnection connection)
    {
        FluentMigratorToolkit.RollbackMigration(serviceProvider, migrationType, connection);
    }

    public virtual void CreateDatabase(string? database)
    {
        using var connection = _connectionService.OpenConnection();
        CreateDatabase(connection, database);
    }
    public virtual void CreateDatabase(int connectionType, string? database)
    {
        using var connection = _connectionService.OpenConnection(connectionType);
        CreateDatabase(connection, database);
    }
    public virtual void CreateDatabase(string connectionString, string? database)
    {
        using var connection = _connectionService.OpenConnection(connectionString);
        CreateDatabase(connection, database);
    }
    public virtual void CreateDatabase(IDbConnection connection, string? database)
    {
        if (string.IsNullOrEmpty(database))
            return;

        var records = connection.Query("SELECT * FROM sys.databases WHERE name = @database", new { database });
        if (!records.Any())
            connection.Query($"CREATE DATABASE {database}");
    }

    public virtual void CreateSchemas(params string[] schemas)
    {
        using var connection = _connectionService.OpenConnection();
        CreateSchemas(connection, schemas);
    }
    public virtual void CreateSchemas(int connectionType, params string[] schemas)
    {
        using var connection = _connectionService.OpenConnection(connectionType);
        CreateSchemas(connection, schemas);
    }
    public virtual void CreateSchemas(string connectionString, params string[] schemas)
    {
        using var connection = _connectionService.OpenConnection(connectionString);
        CreateSchemas(connection, schemas);
    }
    public virtual void CreateSchemas(IDbConnection connection, params string[] schemas)
    {
        if (schemas.Length < 1)
            return;

        foreach (var sch in schemas)
            CreateSchema(connection, sch);
    }

    public virtual void CreateSchema(string schema)
    {
        using var connection = _connectionService.OpenConnection();
        CreateSchema(connection, schema);
    }
    public virtual void CreateSchema(int connectionType, string schema)
    {
        using var connection = _connectionService.OpenConnection(connectionType);
        CreateSchema(connection, schema);
    }
    public virtual void CreateSchema(string connectionString, string schema)
    {
        using var connection = _connectionService.OpenConnection(connectionString);
        CreateSchema(connection, schema);
    }
    public virtual void CreateSchema(IDbConnection connection, string schema)
    {
        var records = connection.Query("SELECT * FROM sys.schemas WHERE name = @schema", new { schema });
        if (!records.Any())
            connection.Query($"CREATE SCHEMA {schema}");
    }
}