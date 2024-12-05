using FluentMigrator.Runner.Initialization;

namespace Common.Data;

public class ConnectionStringReader : IConnectionStringReader
{
    public ConnectionStringReader(string connectionString)
    {
        ConnectionString = connectionString;
    }
    public ConnectionStringReader(IConnectionEssentialService connectionEssentialService, int connectionType)
    {
        ConnectionType = connectionType;
        ConnectionString = connectionEssentialService.GetConnectionString(connectionType);
    }

    /// <summary>Gets or sets the connection string.</summary>
    public string? ConnectionString { get; set; }
    public int? ConnectionType { get; }

    /// <inheritdoc />
    public int Priority { get; } = 300;

    /// <inheritdoc />
    public string? GetConnectionString(string connectionStringOrName)
    {
        return string.IsNullOrEmpty(ConnectionString) ? null : ConnectionString;
    }
}