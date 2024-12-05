using System;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;

using Common.Reflection;

namespace Common.Data;

public static class FluentMigratorToolkit
{
    /// <summary>
    /// Removes given migration from the database including installation record in [VersionInfo] table.
    /// </summary>
    public static void RollbackMigration<T>(IServiceProvider serviceProvider, string connectionString)
        where T : IMigration
    {
        RollbackMigration(serviceProvider, typeof(T), connectionString);
    }
    /// <summary>
    /// Removes given migration from the database including installation record in [VersionInfo] table.
    /// </summary>
    public static void RollbackMigration<T>(IServiceProvider serviceProvider, IDbConnection connection)
        where T : IMigration
    {
        RollbackMigration(serviceProvider, typeof(T), connection);
    }
    /// <summary>
    /// Removes given migration from the database including installation record in [VersionInfo] table.
    /// </summary>
    public static void RollbackMigration(IServiceProvider serviceProvider, Type migrationType, string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        RollbackMigration(serviceProvider, migrationType, connection);
    }
    /// <summary>
    /// Removes given migration from the database including installation record in [VersionInfo] table.
    /// </summary>
    public static void RollbackMigration(IServiceProvider serviceProvider, Type migrationType, IDbConnection connection)
    {
        var instance = ActivatorUtilities.CreateInstance(serviceProvider, migrationType);
        if (instance is not IMigration migration)
            throw new ArgumentException($"Type {migrationType.Name} must implement {nameof(IMigration)} interface.");

        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        runner.Down(migration);

        var attr = migrationType.GetCustomAttributes(typeof(MigrationAttribute), true)
            .FirstOrDefault() as MigrationAttribute;
        if (attr == null)
            throw new ArgumentException($"Type {migrationType.Name} must be marked with {nameof(MigrationAttribute)}.");

        var schema = GetSchema(serviceProvider);
        connection.Execute($@"
DELETE FROM [{schema}].[VersionInfo]
WHERE Version = @Version
", attr);
    }

    private static string GetSchema(IServiceProvider serviceProvider)
    {
        const string defaultSchema = "dbo";

        var convention = serviceProvider.GetRequiredService<IConventionSet>();
        var schemaConvention = convention.SchemaConvention;
        if (schemaConvention == null)
            return defaultSchema;

        var schemaNameConvention = schemaConvention.GetNonPublicInstanceFieldValue<DefaultSchemaNameConvention>("_defaultSchemaNameConvention");
        if (schemaNameConvention == null)
            return defaultSchema;

        var schema = schemaNameConvention.GetNonPublicInstanceFieldValue<string>("_defaultSchemaName");
        return string.IsNullOrEmpty(schema) ? defaultSchema : schema;
    }
}