using System;
using System.Data;

namespace Common.Data;

public interface IConnectionEssentialService
{
    bool IsDefaultConnectionSupported
    {
        get { return false; }
    }

    string GetConnectionString()
    {
        throw new InvalidOperationException("Default connection is not supported by default.");
    }
    string GetConnectionString(int type);

    IDbConnection OpenConnection()
    {
        throw new InvalidOperationException("Default connection is not supported by default.");
    }
    IDbConnection OpenConnection(int type);
    IDbConnection OpenConnection(string connectionString);
}