using System;

namespace Common.Data;

public class DeleteRecordException : RecordException
{
    public DeleteRecordException(string? message)
        : base(false, message)
    {
    }
    public DeleteRecordException(string? message, Exception? inner)
        : base(false, message, inner)
    {
    }
}