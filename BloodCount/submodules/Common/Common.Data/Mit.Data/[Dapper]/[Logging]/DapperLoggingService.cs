using System;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Common.Text.Json;

namespace Common.Data;

public class DapperLoggingService : IDapperLoggingService
{
    private readonly ILogger<DapperLoggingService> logger;
    private readonly DapperServiceLoggingOptions logOptions;

    public DapperLoggingService(ILogger<DapperLoggingService> logger, IOptions<DapperServiceLoggingOptions> logOptions)
    {
        this.logger = logger;
        this.logOptions = logOptions.Value;
    }

    public void LogParams(Type? type, string sql, object? o, Type[]? types,
        IDbConnection? cnn, int? cnnType, IDbTransaction? transaction,
        bool? buffered, string? splitOn, int? commandTimeout, CommandType? commandType, CommandBehavior? commandBehavior,
        string memberName = "", string filePath = "", int lineNumber = 0, string method = "")
    {
        var result = $"DapperService.{method}(…) was called in {filePath}:{lineNumber} ({memberName}).";

        var nl = Environment.NewLine;
        if (logOptions.WriteReturnTypeInfo)
            result += $"{nl}Return type: {Render(type, v => v.FullName)}.";
        if (logOptions.WriteSqlCommand)
            result += $"{nl}Sql command: <![CDATA[{sql}]]>.";
        if (logOptions.WriteParameters)
            result += $"{nl}Parameters: {Render(o, JsonToolkit.Serialize)}.";
        if (logOptions.WriteRecordsetTypesInfo)
            result += $"{nl}Types: {Render(types, v => string.Join(", ", v.Select(t => t.FullName)))}.";
        if (logOptions.WriteConnectionInfo)
        {
            result += $"{nl}Connection: {Render(cnn, JsonToolkit.Serialize)}."
                    + $"{nl}Connection type identifier: {Render(cnnType)}."
                    + $"{nl}Transaction: {Render(transaction, JsonToolkit.Serialize)}."
                    + $"{nl}commandTimeout: {Render(commandTimeout)}.";//важно понимать когда база нагружена
        }
        if (logOptions.WriteDapperSpecialParametersInfo)
        {
            result += $"{nl}Special parameters: {{ buffered: {Render(buffered, v => v.ToString()!.ToLower())}"
                + $", splitOn: {Render(splitOn, v => $"\"{v}\"")}"
                + $", commandTimeout: {Render(commandTimeout)}"
                + $", commandType: {Render(commandType)}"
                + $", commandBehavior: {Render(commandBehavior)} }}.";
        }

        logger.LogInformation(result);
    }


    private static string? Render<T>(T? value)
    {
        return Render<T>(value, v => v!.ToString());
    }
    private static string? Render<T>(T? value, Func<T, string?> render)
    {
        return value == null ? "null" : render(value);
    }
}
