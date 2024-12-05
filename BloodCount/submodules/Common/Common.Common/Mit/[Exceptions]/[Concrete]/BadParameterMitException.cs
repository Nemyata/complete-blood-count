using System;
using Microsoft.Extensions.Logging;

namespace Common;

/// <summary>
/// Специальная ошибка, когда нужно в котроллере сообщить, что на входе были переданы не все или недостоверные параметры.
/// </summary>
public class BadParameterCommonException : CommonException
{
    public BadParameterCommonException(string? message)
        : base(message)
    {
    }
    public BadParameterCommonException(string? message, Exception ex)
        : base(message, ex)
    {
    }

    public BadParameterCommonException(string? message, ILogger? logger = null)
        : base(message, logger)
    {
    }
    public BadParameterCommonException(string? message, Exception inner, ILogger? logger = null)
        : base(message, inner, logger)
    {
    }
}