using System.Collections.Generic;

namespace Common.Data;

public class MigratorSettings
{
    public string? DefaultSchemaName { get; set; }
    public string? Database { get; set; }
    public List<string>? Schemas { get; set; }
}
