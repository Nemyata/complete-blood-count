using System;

namespace Common.Data;

public class AddRecordException : RecordException
{
    public AddRecordException(string? message)
        : base(true, message)
    {
    }
    public AddRecordException(string? message, Exception? inner)
        : base(true, message, inner)
    {
    }
}