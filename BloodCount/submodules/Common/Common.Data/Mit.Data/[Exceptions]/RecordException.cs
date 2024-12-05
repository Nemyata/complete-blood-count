using System;

namespace Common.Data;

public class RecordException : CommonException
{
    public RecordException(bool exist)
        : this(exist, null, null)
    {
    }
    public RecordException(bool exist, string? message)
        : this(exist, message, null)
    {
    }
    public RecordException(bool exist, string? message, Exception? inner)
        : base(message, inner)
    {
        Exist = exist;
    }

    /// <summary>
    /// Запись с указанным идентификатором уже существует в БД
    /// </summary>
    public bool Exist { get; set; }
}