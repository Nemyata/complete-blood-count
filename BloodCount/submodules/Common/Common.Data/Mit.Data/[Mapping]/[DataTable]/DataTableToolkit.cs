using System;
using System.Data;
using System.Linq;

namespace Common.Data;

internal class DataTableToolkit
{
    #region GetColumnNames
    public static string[] GetColumnNames(DataRow row)
    {
        var columns = GetColumnNames(row.Table);
        return columns;
    }
    public static string[] GetColumnNames(DataTable table)
    {
        var columns = table.Columns.Cast<DataColumn>()
            .Select(c => c.ColumnName).ToArray();
        return columns;
    }
    #endregion

    #region UpdateColumnNumberByHeader
    public static int UpdateColumnIndexByHeader<T>(DataTable table, DataField<T> field)
    {
        var headers = GetColumnHeaders(table);

        var number = UpdateColumnIndexByHeader(headers, field);
        return number;
    }
    public static int UpdateColumnIndexByHeader<T>(string?[] headers, DataField<T> field)
    {
        var index = field.Number - 1;
        if (index < 0 && !string.IsNullOrEmpty(field.Header))
            index = Array.IndexOf(headers, field.Header);
        return index;
    }
    #endregion

    #region GetColumnHeaders
    public static string?[] GetColumnHeaders(DataTable table)
    {
        var headers = table.Columns.Cast<DataColumn>()
            .Select((column, index) => table.Rows[0][index].ToString())
            .ToArray();
        return headers;
    }
    #endregion
}
