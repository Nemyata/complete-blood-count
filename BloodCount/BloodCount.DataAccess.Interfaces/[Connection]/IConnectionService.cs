using BloodCount.Domain.Configuration;

using Common.Data;

using System.Data;


namespace BloodCount.DataAccess.Interfaces;

public interface IConnectionService : IConnectionEssentialService
{
    bool IConnectionEssentialService.IsDefaultConnectionSupported
    {
        get { return true; }
    }

    string GetConnectionString(ConnectionType type);
    string GetMainConnectionString();


    IDbConnection OpenConnection(ConnectionType type);
    IDbConnection OpenMainConnection();
}
