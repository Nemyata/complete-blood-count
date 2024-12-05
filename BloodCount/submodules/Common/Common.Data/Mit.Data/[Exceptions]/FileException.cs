using System;

namespace Common.Data;

public class FileException : CommonException
{
    public FileException(string? message)
        : base(message)
    {
    }
    public FileException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}