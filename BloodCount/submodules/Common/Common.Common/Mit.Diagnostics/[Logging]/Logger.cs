using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Common.Diagnostics;

public static class Logger
{
    public static void Exec(this ILogger logger, string title, Action action,
        Action<Exception, string>? onError = null, bool throwOnError = true,
        [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        WriteBegin(logger, title, memberName, filePath, lineNumber);
        try
        {
            action();
            WriteSuccess(logger, title);
        }
        catch (Exception ex)
        {
            WriteError(logger, ex, title);
            if (onError != null)
                onError(ex, title);
            if (throwOnError)
                throw;
        }
    }
    public static void Exec(this ILogger logger, string title, Action<Action<string>> action,
        Action<Exception, string>? onError = null, bool throwOnError = true,
        [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        WriteBegin(logger, title, memberName, filePath, lineNumber);
        try
        {
            action(message => WriteInfo(logger, title, message));
            WriteSuccess(logger, title);
        }
        catch (Exception ex)
        {
            WriteError(logger, ex, title);
            if (onError != null)
                onError(ex, title);
            if (throwOnError)
                throw;
        }
    }
    public static T? Exec<T>(this ILogger logger, string title, Func<T> func,
        Func<Exception, string, T>? onError = null, bool throwOnError = true, T? resultOnError = default,
        [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        WriteBegin(logger, title, memberName, filePath, lineNumber);
        try
        {
            var result = func();
            WriteSuccess(logger, title);
            return result;
        }
        catch (Exception ex)
        {
            WriteError(logger, ex, title);
            if (onError != null)
                return onError(ex, title);
            if (throwOnError)
                throw;
            return resultOnError;
        }
    }
    public static T? Exec<T>(this ILogger logger, string title, Func<Action<string>, T> func,
        Func<Exception, string, T>? onError = null, bool throwOnError = true, T? resultOnError = default,
        [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        WriteBegin(logger, title, memberName, filePath, lineNumber);
        try
        {
            var result = func(message => WriteInfo(logger, title, message));
            WriteSuccess(logger, title);
            return result;
        }
        catch (Exception ex)
        {
            WriteError(logger, ex, title);
            if (onError != null)
                return onError(ex, title);
            if (throwOnError)
                throw;
            return resultOnError;
        }
    }

    public static async Task ExecAsync(this ILogger logger, string title, Func<Task> action,
        Action<Exception, string>? onError = null, bool throwOnError = true,
        [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        WriteBegin(logger, title, memberName, filePath, lineNumber);
        try
        {
            await action();
            WriteSuccess(logger, title);
        }
        catch (Exception ex)
        {
            WriteError(logger, ex, title);
            if (onError != null)
                onError(ex, title);
            if (throwOnError)
                throw;
        }
    }
    public static async Task ExecAsync(this ILogger logger, string title, Func<Action<string>, Task> func,
        Action<Exception, string>? onError = null, bool throwOnError = true,
        [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        WriteBegin(logger, title, memberName, filePath, lineNumber);
        try
        {
            await func(message => WriteInfo(logger, title, message));
            WriteSuccess(logger, title);
        }
        catch (Exception ex)
        {
            WriteError(logger, ex, title);
            if (onError != null)
                onError(ex, title);
            if (throwOnError)
                throw;
        }
    }
    public static async Task<T?> ExecAsync<T>(this ILogger logger, string title, Func<Task<T>> func,
        Func<Exception, string, T>? onError = null, bool throwOnError = true, T? resultOnError = default,
        [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        WriteBegin(logger, title, memberName, filePath, lineNumber);
        try
        {
            var result = await func();
            WriteSuccess(logger, title);
            return result;
        }
        catch (Exception ex)
        {
            WriteError(logger, ex, title);
            if (onError != null)
                return onError(ex, title);
            if (throwOnError)
                throw;
            return resultOnError;
        }
    }
    public static async Task<T?> ExecAsync<T>(this ILogger logger, string title, Func<Action<string>, Task<T>> func,
        Func<Exception, string, T>? onError = null, bool throwOnError = true, T? resultOnError = default,
        [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        WriteBegin(logger, title, memberName, filePath, lineNumber);
        try
        {
            var result = await func(message => WriteInfo(logger, title, message));
            WriteSuccess(logger, title);
            return result;
        }
        catch (Exception ex)
        {
            WriteError(logger, ex, title);
            if (onError != null)
                return onError(ex, title);
            if (throwOnError)
                throw;
            return resultOnError;
        }
    }

    private static void WriteBegin(ILogger logger, string title, string memberName, string filePath, int lineNumber)
    {
        logger.LogInformation($"{title}: старт (in {filePath}:{lineNumber}, {memberName}).");
    }
    private static void WriteInfo(ILogger logger, string message, string info)
    {
        logger.LogInformation($"{message}: {info}");
    }
    private static void WriteSuccess(ILogger logger, string title)
    {
        logger.LogInformation($"{title}: успешное завершение.");
    }
    private static void WriteError(ILogger logger, Exception ex, string title)
    {
        logger.LogError(ex, $"{title}: завершение с ошибкой.");
    }
}