using BloodCount.Domain;
using BloodCount.DataAccess.Interfaces.Main;

using System.Data;

using Dapper;


namespace BloodCount.DataAccess.Implementation.Main;

public class LLMRepository : ILLMRepository
{
    public async Task AddAsync(LLM LLM, IDbConnection connection, IDbTransaction? transaction)
    {
        var SQL = @"
INSERT INTO dbo.LLM 
(Id, FileId, Gender, Hemoglobin, RedBloodCells, Platelets, Leukocytes, ResultML) 
VALUES (@Id, @FileId, @Gender, @Hemoglobin, @RedBloodCells, @Platelets, @Leukocytes, @ResultML)";

        var parameters = new DynamicParameters();
        parameters.Add("Id", LLM.Id);
        parameters.Add("FileId", LLM.FileId);
        parameters.Add("Gender", LLM.Gender);
        parameters.Add("Hemoglobin", LLM.Hemoglobin);
        parameters.Add("RedBloodCells", LLM.RedBloodCells);
        parameters.Add("Platelets", LLM.Platelets);
        parameters.Add("Leukocytes", LLM.Leukocytes);
        parameters.Add("ResultML", LLM.ResultML);

        await connection.ExecuteAsync(SQL, parameters, transaction);
    }
}