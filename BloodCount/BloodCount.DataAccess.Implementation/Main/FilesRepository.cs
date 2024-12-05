using BloodCount.Domain;
using BloodCount.DataAccess.Interfaces.Main;

using System.Data;

using Dapper;


namespace BloodCount.DataAccess.Implementation.Main;

public class FilesRepository : IFilesRepository
{
    public async Task AddAsync(Files files, IDbConnection connection, IDbTransaction? transaction)
    {
        var SQL = "INSERT INTO dbo.Files (Id, FileName, OCR) VALUES (@Id, @FileName, @OCR)";

        var parameters = new DynamicParameters();
        parameters.Add("Id", files.Id);
        parameters.Add("FileName", files.FileName);
        parameters.Add("OCR", files.OCR);

        await connection.ExecuteAsync(SQL, parameters, transaction);
    }
}
