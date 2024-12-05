using BloodCount.Domain.Configuration;
using BloodCount.DataAccess.Interfaces;

using System.Data;
using System.ComponentModel;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using DocumentFormat.OpenXml.Spreadsheet;


namespace BloodCount.DataAccess.Implementation;

public class ConnectionService(IOptions<ConnectionStrings> databaseOptions) : IConnectionService
{
    private readonly ConnectionStrings _databaseOptions = databaseOptions.Value;

    public string GetConnectionString()
    {
        return GetConnectionString(ConnectionType.Main);
    }

    public string GetConnectionString(ConnectionType type)
    {
        var connection = type switch
        {
            ConnectionType.Main => GetMainConnectionString(),
            _ => throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ConnectionType))
        };
        return connection;
    }

    public string GetConnectionString(int type)
    {
        return GetConnectionString((ConnectionType)type);
    }

    public string GetMainConnectionString()
    {
        return _databaseOptions.Main;
    }

    public IDbConnection OpenConnection(ConnectionType type)
    {
        var connection = type switch
        {
            ConnectionType.Main => OpenMainConnection(),
            _ => throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ConnectionType))
        };
        return connection;
    }

    public IDbConnection OpenConnection(int type)
    {
        return OpenConnection((ConnectionType)type);
    }

    public IDbConnection OpenConnection(string connectionString)
    {
        var connection = OpenSqlConnection(connectionString);
        return connection; ;
    }

    public IDbConnection OpenMainConnection()
    {
        var connection = OpenSqlConnection(_databaseOptions.Main);
        return connection;
    }

    public IDbConnection OpenSqlConnection(string connectionString)
    {
        var connection = new SqlConnection(connectionString);
        if (connection.State != ConnectionState.Open)
            connection.Open();
        return connection;
    }
}
