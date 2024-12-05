using System;

namespace Common.Data;

public class FolderException : CommonException
{
    public FolderException(bool exist)
        : this(exist, null, null)
    {
    }
    public FolderException(bool exist, string? message)
        : this(exist, message, null)
    {
    }
    public FolderException(bool exist, string? message, Exception? inner)
        : base(message, inner)
    {
        Exist = exist;
    }

    /// <summary>
    /// Наличие папки на указанном ресурсе
    /// </summary>
    public bool Exist { get; set; }
}