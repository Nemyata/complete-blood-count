// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable UnusedMember.Local

using Dapper;
using System;
using System.Data;

namespace Common.Data;

public interface IDapperLoggingService
{
    void LogParams(string sql, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(null, sql, o, cnn, cnnType, transaction, commandTimeout, commandType,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<T>(string sql, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(T), sql, o, cnn, cnnType, transaction, commandTimeout, commandType,
            memberName, filePath, lineNumber, method);
    }
    void LogParams(Type? type, string sql, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(type, sql, o, null, cnn, cnnType, transaction, null, null, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }

    void LogParams(CommandDefinition command, IDbConnection? cnn, int? cnnType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(null, command, null, cnn, cnnType,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<T>(CommandDefinition command, IDbConnection? cnn, int? cnnType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(T), command, null, cnn, cnnType,
            memberName, filePath, lineNumber, method);
    }
    void LogParams(Type type, CommandDefinition command, IDbConnection? cnn, int? cnnType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(type, command, null, cnn, cnnType,
            memberName, filePath, lineNumber, method);
    }
    void LogParams(CommandDefinition command, CommandBehavior commandBehavior, IDbConnection? cnn, int? cnnType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(null, command, commandBehavior, cnn, cnnType,
            memberName, filePath, lineNumber, method);
    }
    void LogParams(Type? type, CommandDefinition command, CommandBehavior? commandBehavior, IDbConnection? cnn, int? cnnType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(type, command.CommandText, command.Parameters, null, cnn, cnnType, command.Transaction, null, null, command.CommandTimeout, command.CommandType, commandBehavior,
            memberName, filePath, lineNumber, method);
    }

    void LogParams<TFirst, TSecond, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TReturn> map, IDbConnection? cnn, int? cnnType, string splitOn,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), command.CommandText, command.Parameters, new[] { typeof(TFirst), typeof(TSecond) }, cnn, cnnType, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TFirst, TSecond, TThird, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, IDbConnection? cnn, int? cnnType, string splitOn,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), command.CommandText, command.Parameters, new[] { typeof(TFirst), typeof(TSecond), typeof(TThird) }, cnn, cnnType, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TFirst, TSecond, TThird, TFourth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, IDbConnection? cnn, int? cnnType, string splitOn,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), command.CommandText, command.Parameters, new[] { typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth) }, cnn, cnnType, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, IDbConnection? cnn, int? cnnType, string splitOn,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), command.CommandText, command.Parameters, new[] { typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth), typeof(TFifth) }, cnn, cnnType, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, IDbConnection? cnn, int? cnnType, string splitOn,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), command.CommandText, command.Parameters, new[] { typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth), typeof(TFifth), typeof(TSixth) }, cnn, cnnType, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, IDbConnection? cnn, int? cnnType, string splitOn,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), command.CommandText, command.Parameters, new[] { typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth), typeof(TFifth), typeof(TSixth), typeof(TSeventh) }, cnn, cnnType, command.Transaction, command.Buffered, splitOn, command.CommandTimeout, command.CommandType, null,
            memberName, filePath, lineNumber, method);
    }

    void LogParams<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool buffered, string splitOn, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), sql, o, new[] { typeof(TFirst), typeof(TSecond) }, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool buffered, string splitOn, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), sql, o, new[] { typeof(TFirst), typeof(TSecond), typeof(TThird) }, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool buffered, string splitOn, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), sql, o, new[] { typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth) }, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool buffered, string splitOn, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), sql, o, new[] { typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth), typeof(TFifth) }, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool buffered, string splitOn, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), sql, o, new[] { typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth), typeof(TFifth), typeof(TSixth) }, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool buffered, string splitOn, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), sql, o, new[] { typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth), typeof(TFifth), typeof(TSixth), typeof(TSeventh) }, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<TReturn>(string sql, Type[] types, Func<object[], TReturn> map, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool buffered, string splitOn, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(TReturn), sql, o, types, cnn, cnnType, transaction, buffered, splitOn, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }

    void LogParams(string sql, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool buffered, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(null, sql, o, null, cnn, cnnType, transaction, buffered, null, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams<T>(string sql, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool buffered, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(typeof(T), sql, o, null, cnn, cnnType, transaction, buffered, null, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }
    void LogParams(Type? type, string sql, object? o, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool buffered, int? commandTimeout, CommandType? commandType,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        LogParams(type, sql, o, null, cnn, cnnType, transaction, buffered, null, commandTimeout, commandType, null,
            memberName, filePath, lineNumber, method);
    }

    void LogParams(Type? type, string sql, object? o, Type[]? types, IDbConnection? cnn, int? cnnType, IDbTransaction? transaction, bool? buffered, string? splitOn, int? commandTimeout, CommandType? commandType, CommandBehavior? commandBehavior,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "");
}
