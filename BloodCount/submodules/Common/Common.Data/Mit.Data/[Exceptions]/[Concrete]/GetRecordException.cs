using System;

namespace Common.Data;

public class GetRecordException : RecordException
{
    public GetRecordException(string? message)
        : base(false, message)
    {
    }
    public GetRecordException(string? message, Exception? inner)
        : base(false, message, inner)
    {
    }
}