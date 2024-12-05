using System;

namespace Common.Data;

public class UpdateRecordException : RecordException
{
    public UpdateRecordException(bool exist)
        : base(exist)
    {
    }
    public UpdateRecordException(bool exist, string? message)
        : base(exist, message)
    {
    }
    public UpdateRecordException(bool exist, string? message, Exception? inner)
        : base(exist, message, inner)
    {
    }
}