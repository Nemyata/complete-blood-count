using BloodCount.Domain;

using System.Data;


namespace BloodCount.DataAccess.Interfaces.Main;

public interface ILLMRepository
{
    Task AddAsync(LLM LLM, IDbConnection connection, IDbTransaction? transaction);
}