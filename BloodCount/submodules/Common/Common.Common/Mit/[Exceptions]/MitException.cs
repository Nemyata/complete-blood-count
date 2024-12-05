using System;
using Microsoft.Extensions.Logging;

namespace Common;

/// <summary>
/// Базовый класс для всех исключений в управлении 
/// </summary>
public class CommonException : Exception
{
    public CommonException(string? message)
        : base(message)
    {
    }
    public CommonException(string? message, Exception? ex)
        : base(message, ex)
    {
    }

    public CommonException(string? message, ILogger logger)
        : base(message)
    {
        logger.LogError(message);
    }
    public CommonException(string? message, Exception? ex, ILogger logger)
        : base(message, ex)
    {
        logger.LogError(ex, message);
    }
}