using System.Data;

using BloodCount.Domain;


namespace BloodCount.DataAccess.Interfaces.Main;

public interface IFilesRepository
{
    Task AddAsync(Files files, IDbConnection connection, IDbTransaction? transaction);
}