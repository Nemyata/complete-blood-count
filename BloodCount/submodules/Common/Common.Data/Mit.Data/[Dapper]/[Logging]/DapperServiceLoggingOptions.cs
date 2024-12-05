// ReSharper disable UnusedMember.Global

namespace Common.Data;

public class DapperServiceLoggingOptions
{
    public bool WriteReturnTypeInfo { get; set; } = false;
    public bool WriteSqlCommand { get; set; } = true;
    public bool WriteParameters { get; set; } = false;
    public bool WriteRecordsetTypesInfo { get; set; } = false;
    public bool WriteConnectionInfo { get; set; } = false;
    public bool WriteDapperSpecialParametersInfo { get; set; } = false;
}
