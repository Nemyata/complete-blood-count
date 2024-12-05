/// За основу файла взята библиотека Dapper

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dapper;

namespace Common.Data;

/// <summary>Wrapper over <see cref="Dapper"/>, a light weight object mapper for ADO.NET.
/// In addition to main <see cref="Dapper"/> functionality it introduces features like
/// logging of method's parameters and its caller
/// using user provided implementation of <see cref="IDapperLoggingService"/>,
/// construction of short-lived connection via its type identifier (<see cref="int"/>)
/// using implementation of <see cref="IConnectionEssentialService"/> provided by user too.</summary>
public class DapperService : IDapperService
{
    private readonly ILogger<DapperService> logger;
    private readonly IDapperLoggingService _dapperLoggingService;
    private readonly IConnectionEssentialService connectionService;

    public DapperService(ILogger<DapperService> logger, IDapperLoggingService dapperLoggingService, IConnectionEssentialService connectionService)
    {
        this.logger = logger;
        this._dapperLoggingService = dapperLoggingService;
        this.connectionService = connectionService;
    }

    /// <summary>Execute a query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public Task<IEnumerable<object>> QueryAsync(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public Task<IEnumerable<object>> QueryAsync(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "QueryAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync(command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public Task<object> QueryFirstAsync(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "QueryFirstAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstAsync(command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public Task<object> QueryFirstOrDefaultAsync(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "QueryFirstOrDefaultAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstOrDefaultAsync(command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public Task<object> QuerySingleAsync(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "QuerySingleAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleAsync(command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public Task<object> QuerySingleOrDefaultAsync(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "QuerySingleOrDefaultAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleOrDefaultAsync(command));
        return result;
    }
    /// <summary>Execute a query asynchronously using Task.</summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of <typeparamref name="T" />; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<T>(sql, param, transaction, commandTimeout == null ? cnn?.ConnectionTimeout : commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    public Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirstAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirstOrDefaultAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    public Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingleAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingleOrDefaultAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    public Task<object> QueryFirstAsync(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirstAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    public Task<object> QueryFirstOrDefaultAsync(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirstOrDefaultAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstOrDefaultAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    public Task<object> QuerySingleAsync(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingleAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    public Task<object> QuerySingleOrDefaultAsync(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingleOrDefaultAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleOrDefaultAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <c>null</c>.</exception>
    public Task<IEnumerable<object>> QueryAsync(Type type, string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(type, sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync(type, sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <c>null</c>.</exception>
    public Task<object> QueryFirstAsync(Type type, string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(type, sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirstAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstAsync(type, sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <c>null</c>.</exception>
    public Task<object> QueryFirstOrDefaultAsync(Type type, string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(type, sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirstOrDefaultAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstOrDefaultAsync(type, sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <c>null</c>.</exception>
    public Task<object> QuerySingleAsync(Type type, string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(type, sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingleAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleAsync(type, sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <c>null</c>.</exception>
    public Task<object> QuerySingleOrDefaultAsync(Type type, string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(type, sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingleOrDefaultAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleOrDefaultAsync(type, sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a query asynchronously using Task.</summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <returns>
    /// A sequence of data of <typeparamref name="T" />; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "QueryAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<T>(command));
        return result;
    }
    /// <summary>Execute a query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="command">The command used to query on this connection.</param>
    public Task<IEnumerable<object>> QueryAsync(Type type, CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(type, command, cnn, cnnType, memberName, filePath, lineNumber, "QueryAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync(type, command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="command">The command used to query on this connection.</param>
    public Task<object> QueryFirstAsync(Type type, CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(type, command, cnn, cnnType, memberName, filePath, lineNumber, "QueryFirstAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstAsync(type, command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    public Task<T> QueryFirstAsync<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "QueryFirstAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstAsync<T>(command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="command">The command used to query on this connection.</param>
    public Task<object> QueryFirstOrDefaultAsync(Type type, CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(type, command, cnn, cnnType, memberName, filePath, lineNumber, "QueryFirstOrDefaultAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstOrDefaultAsync(type, command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    public Task<T> QueryFirstOrDefaultAsync<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "QueryFirstOrDefaultAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstOrDefaultAsync<T>(command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="command">The command used to query on this connection.</param>
    public Task<object> QuerySingleAsync(Type type, CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(type, command, cnn, cnnType, memberName, filePath, lineNumber, "QuerySingleAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleAsync(type, command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    public Task<T> QuerySingleAsync<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "QuerySingleAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleAsync<T>(command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="command">The command used to query on this connection.</param>
    public Task<object> QuerySingleOrDefaultAsync(Type type, CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(type, command, cnn, cnnType, memberName, filePath, lineNumber, "QuerySingleOrDefaultAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleOrDefaultAsync(type, command));
        return result;
    }
    /// <summary>Execute a single-row query asynchronously using Task.</summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    public Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "QuerySingleOrDefaultAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleOrDefaultAsync<T>(command));
        return result;
    }
    /// <summary>Execute a command asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The number of rows affected.</returns>
    public Task<int> ExecuteAsync(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "ExecuteAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute a command asynchronously using Task.</summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute on this connection.</param>
    /// <returns>The number of rows affected.</returns>
    public Task<int> ExecuteAsync(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "ExecuteAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.ExecuteAsync(command));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 2 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 2 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TReturn> map, IDbConnection? cnn = null, int? cnnType = null, string splitOn = "Id", [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TReturn>(command, map, cnn, cnnType, splitOn, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TReturn>(command, map, splitOn));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 3 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TThird, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TThird, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 3 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, IDbConnection? cnn = null, int? cnnType = null, string splitOn = "Id", [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TReturn>(command, map, cnn, cnnType, splitOn, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TThird, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TThird, TReturn>(command, map, splitOn));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 4 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 4 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, IDbConnection? cnn = null, int? cnnType = null, string splitOn = "Id", [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TReturn>(command, map, cnn, cnnType, splitOn, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(command, map, splitOn));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 5 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 5 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, IDbConnection? cnn = null, int? cnnType = null, string splitOn = "Id", [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(command, map, cnn, cnnType, splitOn, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(command, map, splitOn));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 6 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
    /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 6 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
    /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, IDbConnection? cnn = null, int? cnnType = null, string splitOn = "Id", [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(command, map, cnn, cnnType, splitOn, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(command, map, splitOn));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 7 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
    /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
    /// <typeparam name="TSeventh">The seventh type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with 7 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
    /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
    /// <typeparam name="TSeventh">The seventh type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, IDbConnection? cnn = null, int? cnnType = null, string splitOn = "Id", [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(command, map, cnn, cnnType, splitOn, memberName, filePath, lineNumber, "QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(command, map, splitOn));
        return result;
    }
    /// <summary>
    /// Perform an asynchronous multi-mapping query with an arbitrary number of input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="types">Array of types in the recordset.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public Task<IEnumerable<TReturn>> QueryAsync<TReturn>(string sql, Type[] types, Func<object[], TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<TReturn>(sql, types, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryAsync<TReturn>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryAsync<TReturn>(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute a command that returns multiple result sets, and access each in turn.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    public Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryMultipleAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute a command that returns multiple result sets, and access each in turn.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command to execute for this query.</param>
    public async Task<SqlMapper.GridReader> QueryMultipleAsync(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "QueryMultipleAsync");
        var result = await ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.QueryMultipleAsync(command));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader" />.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An <see cref="T:System.Data.IDataReader" /> that can be used to iterate over the results of the SQL query.</returns>
    /// <remarks>
    /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="T:System.Data.DataTable" />
    /// or <see cref="T:DataSet" />.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// DataTable table = new DataTable("MyTable");
    /// using (var reader = ExecuteReader(cnn, sql, param))
    /// {
    ///     table.Load(reader);
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public Task<IDataReader> ExecuteReaderAsync(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "ExecuteReaderAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL and return a <see cref="T:System.Data.Common.DbDataReader" />.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    public Task<DbDataReader> ExecuteReaderAsync(string sql, object? param = null, DbConnection cnn = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, null, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "ExecuteReaderAsync");
        var result = ExecAsync(cnn, null, (DbConnection connection) => connection.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader" />.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <returns>An <see cref="T:System.Data.IDataReader" /> that can be used to iterate over the results of the SQL query.</returns>
    /// <remarks>
    /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="T:System.Data.DataTable" />
    /// or <see cref="T:DataSet" />.
    /// </remarks>
    public Task<IDataReader> ExecuteReaderAsync(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "ExecuteReaderAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.ExecuteReaderAsync(command));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL and return a <see cref="T:System.Data.Common.DbDataReader" />.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    public Task<DbDataReader> ExecuteReaderAsync(CommandDefinition command, DbConnection cnn = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, null, memberName, filePath, lineNumber, "ExecuteReaderAsync");
        var result = ExecAsync(cnn, null, (DbConnection connection) => connection.ExecuteReaderAsync(command));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader" />.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="commandBehavior">The <see cref="T:System.Data.CommandBehavior" /> flags for this reader.</param>
    /// <returns>An <see cref="T:System.Data.IDataReader" /> that can be used to iterate over the results of the SQL query.</returns>
    /// <remarks>
    /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="T:System.Data.DataTable" />
    /// or <see cref="T:DataSet" />.
    /// </remarks>
    public Task<IDataReader> ExecuteReaderAsync(CommandDefinition command, CommandBehavior commandBehavior, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, commandBehavior, cnn, cnnType, memberName, filePath, lineNumber, "ExecuteReaderAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.ExecuteReaderAsync(command, commandBehavior));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL and return a <see cref="T:System.Data.Common.DbDataReader" />.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="commandBehavior">The <see cref="T:System.Data.CommandBehavior" /> flags for this reader.</param>
    public Task<DbDataReader> ExecuteReaderAsync(CommandDefinition command, CommandBehavior commandBehavior, DbConnection cnn = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, commandBehavior, cnn, null, memberName, filePath, lineNumber, "ExecuteReaderAsync");
        var result = ExecAsync(cnn, null, (DbConnection connection) => connection.ExecuteReaderAsync(command, commandBehavior));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The first cell returned, as <see cref="T:System.Object" />.</returns>
    public Task<object> ExecuteScalarAsync(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "ExecuteScalarAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The first cell returned, as <typeparamref name="T" />.</returns>
    public Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "ExecuteScalarAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <returns>The first cell selected as <see cref="T:System.Object" />.</returns>
    public Task<object> ExecuteScalarAsync(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "ExecuteScalarAsync");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.ExecuteScalarAsync(command));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <returns>The first cell selected as <typeparamref name="T" />.</returns>
    public Task<T> ExecuteScalarAsync<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "ExecuteScalarAsync<T>");
        var result = ExecAsync(cnn, cnnType, (IDbConnection connection) => connection.ExecuteScalarAsync<T>(command));
        return result;
    }
    /// <summary>
    /// Execute a query asynchronously using <see cref="T:System.Collections.Generic.IAsyncEnumerable`1" />.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>A sequence of data of dynamic data</returns>
    public IAsyncEnumerable<object> QueryUnbufferedAsync(string sql, object? param = null, DbConnection cnn = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, null, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryUnbufferedAsync");
        var result = Exec(cnn, null, (DbConnection connection) => connection.QueryUnbufferedAsync(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute a query asynchronously using <see cref="T:System.Collections.Generic.IAsyncEnumerable`1" />.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of <typeparamref name="T" />; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public IAsyncEnumerable<T> QueryUnbufferedAsync<T>(string sql, object? param = null, DbConnection cnn = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, null, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryUnbufferedAsync<T>");
        var result = Exec(cnn, null, (DbConnection connection) => connection.QueryUnbufferedAsync<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute parameterized SQL.</summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The number of rows affected.</returns>
    public int Execute(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "Execute");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Execute(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>Execute parameterized SQL.</summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute on this connection.</param>
    /// <returns>The number of rows affected.</returns>
    public int Execute(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "Execute");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Execute(command));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The first cell selected as <see cref="T:System.Object" />.</returns>
    public object ExecuteScalar(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "ExecuteScalar");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.ExecuteScalar(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>The first cell returned, as <typeparamref name="T" />.</returns>
    public T ExecuteScalar<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "ExecuteScalar<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <returns>The first cell selected as <see cref="T:System.Object" />.</returns>
    public object ExecuteScalar(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "ExecuteScalar");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.ExecuteScalar(command));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <returns>The first cell selected as <typeparamref name="T" />.</returns>
    public T ExecuteScalar<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "ExecuteScalar<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.ExecuteScalar<T>(command));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader" />.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An <see cref="T:System.Data.IDataReader" /> that can be used to iterate over the results of the SQL query.</returns>
    /// <remarks>
    /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="T:System.Data.DataTable" />
    /// or <see cref="T:DataSet" />.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// DataTable table = new DataTable("MyTable");
    /// using (var reader = ExecuteReader(cnn, sql, param))
    /// {
    ///     table.Load(reader);
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public IDataReader ExecuteReader(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "ExecuteReader");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.ExecuteReader(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader" />.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <returns>An <see cref="T:System.Data.IDataReader" /> that can be used to iterate over the results of the SQL query.</returns>
    /// <remarks>
    /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="T:System.Data.DataTable" />
    /// or <see cref="T:DataSet" />.
    /// </remarks>
    public IDataReader ExecuteReader(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "ExecuteReader");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.ExecuteReader(command));
        return result;
    }
    /// <summary>
    /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader" />.
    /// </summary>
    /// <param name="cnn">Optional connection to execute on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to execute on.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="commandBehavior">The <see cref="T:System.Data.CommandBehavior" /> flags for this reader.</param>
    /// <returns>An <see cref="T:System.Data.IDataReader" /> that can be used to iterate over the results of the SQL query.</returns>
    /// <remarks>
    /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="T:System.Data.DataTable" />
    /// or <see cref="T:DataSet" />.
    /// </remarks>
    public IDataReader ExecuteReader(CommandDefinition command, CommandBehavior commandBehavior, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, commandBehavior, cnn, cnnType, memberName, filePath, lineNumber, "ExecuteReader");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.ExecuteReader(command, commandBehavior));
        return result;
    }
    /// <summary>
    /// Return a sequence of dynamic objects with properties matching the columns.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public IEnumerable<object> Query(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, buffered, commandTimeout, commandType, memberName, filePath, lineNumber, "Query");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query(sql, param, transaction, buffered, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public object QueryFirst(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirst");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QueryFirst(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public object QueryFirstOrDefault(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirstOrDefault");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public object QuerySingle(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingle");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QuerySingle(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Return a dynamic object with properties matching the columns.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
    public object QuerySingleOrDefault(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingleOrDefault");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleOrDefault(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="buffered">Whether to buffer results in memory.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public IEnumerable<T> Query<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, buffered, commandTimeout, commandType, memberName, filePath, lineNumber, "Query<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public T QueryFirst<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirst<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QueryFirst<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public T QueryFirstOrDefault<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirstOrDefault<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public T QuerySingle<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingle<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a single-row query, returning the data typed as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public T QuerySingleOrDefault<T>(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<T>(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingleOrDefault<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a single-row query, returning the data typed as <paramref name="type" />.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="buffered">Whether to buffer results in memory.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <c>null</c>.</exception>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public IEnumerable<object> Query(Type type, string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(type, sql, param, cnn, cnnType, transaction, buffered, commandTimeout, commandType, memberName, filePath, lineNumber, "Query");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query(type, sql, param, transaction, buffered, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a single-row query, returning the data typed as <paramref name="type" />.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <c>null</c>.</exception>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public object QueryFirst(Type type, string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(type, sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirst");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QueryFirst(type, sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a single-row query, returning the data typed as <paramref name="type" />.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <c>null</c>.</exception>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public object QueryFirstOrDefault(Type type, string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(type, sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryFirstOrDefault");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstOrDefault(type, sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a single-row query, returning the data typed as <paramref name="type" />.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <c>null</c>.</exception>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public object QuerySingle(Type type, string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(type, sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingle");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QuerySingle(type, sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a single-row query, returning the data typed as <paramref name="type" />.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="type">The type to return.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <c>null</c>.</exception>
    /// <returns>
    /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public object QuerySingleOrDefault(Type type, string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(type, sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QuerySingleOrDefault");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleOrDefault(type, sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <returns>
    /// A sequence of data of <typeparamref name="T" />; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public IEnumerable<T> Query<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "Query<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query<T>(command));
        return result;
    }
    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <returns>
    /// A single instance or null of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public T QueryFirst<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "QueryFirst<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QueryFirst<T>(command));
        return result;
    }
    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <returns>
    /// A single or null instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public T QueryFirstOrDefault<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "QueryFirstOrDefault<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QueryFirstOrDefault<T>(command));
        return result;
    }
    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <returns>
    /// A single instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public T QuerySingle<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "QuerySingle<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QuerySingle<T>(command));
        return result;
    }
    /// <summary>
    /// Executes a query, returning the data typed as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command used to query on this connection.</param>
    /// <returns>
    /// A single instance of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
    /// </returns>
    public T QuerySingleOrDefault<T>(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams<T>(command, cnn, cnnType, memberName, filePath, lineNumber, "QuerySingleOrDefault<T>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QuerySingleOrDefault<T>(command));
        return result;
    }
    /// <summary>
    /// Execute a command that returns multiple result sets, and access each in turn.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    public SqlMapper.GridReader QueryMultiple(string sql, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams(sql, param, cnn, cnnType, transaction, commandTimeout, commandType, memberName, filePath, lineNumber, "QueryMultiple");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QueryMultiple(sql, param, transaction, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Execute a command that returns multiple result sets, and access each in turn.
    /// </summary>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="command">The command to execute for this query.</param>
    public SqlMapper.GridReader QueryMultiple(CommandDefinition command, IDbConnection? cnn = null, int? cnnType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        _dapperLoggingService.LogParams(command, cnn, cnnType, memberName, filePath, lineNumber, "QueryMultiple");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.QueryMultiple(command));
        return result;
    }
    /// <summary>
    /// Perform a multi-mapping query with 2 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<TFirst, TSecond, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "Query<TFirst, TSecond, TReturn>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query<TFirst, TSecond, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform a multi-mapping query with 3 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "Query<TFirst, TSecond, TThird, TReturn>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query<TFirst, TSecond, TThird, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform a multi-mapping query with 4 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "Query<TFirst, TSecond, TThird, TFourth, TReturn>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query<TFirst, TSecond, TThird, TFourth, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform a multi-mapping query with 5 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform a multi-mapping query with 6 input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
    /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform a multi-mapping query with 7 input types. If you need more types -&gt; use Query with Type[] parameter.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TFourth">The fourth type in the recordset.</typeparam>
    /// <typeparam name="TFifth">The fifth type in the recordset.</typeparam>
    /// <typeparam name="TSixth">The sixth type in the recordset.</typeparam>
    /// <typeparam name="TSeventh">The seventh type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(sql, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }
    /// <summary>
    /// Perform a multi-mapping query with an arbitrary number of input types.
    /// This returns a single type, combined from the raw types via <paramref name="map" />.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <param name="cnn">Optional connection to query on.</param>
    /// <param name="cnnType">Optional connection type identifier to initialize connection to query on.</param>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="types">Array of types in the recordset.</param>
    /// <param name="map">The function to map row types to the return type.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="splitOn">The field we should split and read the second object from (default: "Id").</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <returns>An enumerable of <typeparamref name="TReturn" />.</returns>
    public IEnumerable<TReturn> Query<TReturn>(string sql, Type[] types, Func<object[], TReturn> map, object? param = null, IDbConnection? cnn = null, int? cnnType = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        commandTimeout = commandTimeout != null ? commandTimeout : cnn?.ConnectionTimeout;
        _dapperLoggingService.LogParams<TReturn>(sql, types, map, param, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, memberName, filePath, lineNumber, "Query<TReturn>");
        var result = Exec(cnn, cnnType, (IDbConnection connection) => connection.Query<TReturn>(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        return result;
    }

    private IDbConnection GetDbConnection(IDbConnection? cnn, int? cnnType, out bool created)
    {
        if (cnn != null)
        {
            created = false;
            return cnn;
        }

        if (cnnType != null)
        {
            created = true;
            return connectionService.OpenConnection(cnnType.Value);
        }

        if (!connectionService.IsDefaultConnectionSupported)
            throw new InvalidOperationException($"Default connection is not supported by implementation {connectionService.GetType()} of {nameof(IConnectionEssentialService)} interface.");

        created = true;
        return connectionService.OpenConnection();
    }

    private T Exec<T>(IDbConnection? cnn, int? cnnType, Func<IDbConnection, T> func)
    {
        var created = false;
        IDbConnection? connection = null;
        try
        {
            connection = GetDbConnection(cnn, cnnType, out created);
            var result = func(connection);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while accessing database.");
            throw;
        }
        finally
        {
            if (created && connection != null)
                connection.Dispose();
        }
    }
    private T Exec<T>(IDbConnection? cnn, int? cnnType, Func<DbConnection, T> func)
    {
        var created = false;
        DbConnection? connection = null;
        try
        {
            connection = (DbConnection)GetDbConnection(cnn, cnnType, out created);
            var result = func(connection);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while accessing database.");
            throw;
        }
        finally
        {
            if (created && connection != null)
                connection.Dispose();
        }
    }

    private async Task<T> ExecAsync<T>(IDbConnection? cnn, int? cnnType, Func<IDbConnection, Task<T>> func)
    {
        var created = false;
        IDbConnection? connection = null;
        try
        {
            connection = GetDbConnection(cnn, cnnType, out created);
            var result = await func(connection);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while accessing database.");
            throw;
        }
        finally
        {
            if (created && connection != null)
                connection.Dispose();
        }
    }
    private async Task<T> ExecAsync<T>(IDbConnection? cnn, int? cnnType, Func<DbConnection, Task<T>> func)
    {
        var created = false;
        DbConnection? connection = null;
        try
        {
            connection = (DbConnection)GetDbConnection(cnn, cnnType, out created);
            var result = await func(connection);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while accessing database.");
            throw;
        }
        finally
        {
            if (created && connection != null)
                connection.Dispose();
        }
    }
}
