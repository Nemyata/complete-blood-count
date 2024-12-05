using BloodCount.Domain.Configuration;


namespace BloodCount.DataAccess.Interfaces.Python;

public interface IPythonCallback
{
    Task<string> CallPythonAsync(PythonType script, string args);
}