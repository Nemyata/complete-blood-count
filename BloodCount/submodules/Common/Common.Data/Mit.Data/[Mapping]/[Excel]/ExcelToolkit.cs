using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;
using Common.Linq;
using OfficeOpenXml;

namespace Common.Data;

internal class ExcelToolkit
{
    public static string GetLetter(int columnNumber)
    {
        var result = "";

        while (columnNumber > 0)
        {
            var modulo = (columnNumber - 1) % 26;
            result = Convert.ToChar('A' + modulo) + result;
            columnNumber = (columnNumber - modulo) / 26;
        }

        return result;
    }
    public static int GetNumber(string? letter)
    {
        if (string.IsNullOrEmpty(letter))
            return -1;

        letter = letter.ToUpperInvariant();

        var sum = 0;
        foreach (char t in letter)
        {
            sum *= 26;
            sum += (t - 'A') + 1;
        }

        return sum;
    }

    #region GetColumnNames
    public static string[] GetColumnNames(IXLRow row)
    {
        var columns = GetColumnNames(row.Worksheet);
        return columns;
    }
    public static string[] GetColumnNames(IXLWorksheet sheet)
    {
        var columns = sheet.Columns()
            .Select(c => c.RangeAddress.FirstAddress.ColumnLetter).ToArray();
        return columns;
    }

    public static string[] GetColumnNames(ExcelWorksheet sheet)
    {
        var columnCount = sheet.Dimension.End.Column;
        var columns = Enumerable.Range(1, columnCount)
            .Select(ExcelCellAddress.GetColumnLetter).ToArray();
        return columns;
    }
    #endregion

    #region GetRows
    public static IEnumerable<IXLRow> GetRows(IXLWorksheet sheet)
    {
        var rows = sheet.RowsUsed().ToArray();
        return rows;
    }
    public static IEnumerable<int> GetRows(ExcelWorksheet sheet)
    {
        if (sheet.Dimension == null)
            return Array.Empty<int>();

        var rows = Enumerable.Range(sheet.Dimension.Start.Row, sheet.Dimension.End.Row);
        return rows;
    }
    #endregion

    #region UpdateColumnNumberByHeader
    public static int UpdateColumnNumberByHeader<T>(IXLWorksheet sheet, DataField<T> field)
    {
        return !string.IsNullOrEmpty(field.Header) ? GetColumnNumberByHeader(sheet, field.Header) ?? 0 : 0;
    }
    public static int UpdateColumnNumberByHeader<T>(ExcelWorksheet sheet, DataField<T> field)
    {
        return !string.IsNullOrEmpty(field.Header) ? GetColumnNumberByHeader(sheet, field.Header) ?? 0 : 0;
    }
    #endregion
    #region CheckColumnLetterByHeader
    public static string CheckColumnLetterByHeader<T>(string letter, IXLWorksheet sheet, DataField<T> field)
    {
        if (!string.IsNullOrEmpty(letter) || string.IsNullOrEmpty(field.Header))
            return letter;

        var column = GetColumnLetterByHeader(sheet, field.Header);
        return !string.IsNullOrEmpty(column) ? column : letter;
    }
    public static string CheckColumnLetterByHeader<T>(string letter, ExcelWorksheet sheet, DataField<T> field)
    {
        if (!string.IsNullOrEmpty(letter) || string.IsNullOrEmpty(field.Header))
            return letter;

        var column = GetColumnLetterByHeader(sheet, field.Header);
        return !string.IsNullOrEmpty(column) ? column : letter;
    }
    #endregion

    #region GetColumnLetterByHeader
    public static string GetColumnLetterByHeader(IXLWorksheet sheet, string columnHeader)
    {
        var cell = sheet.FirstRowUsed().CellsUsed(c => c.GetString() == columnHeader).FirstOrDefault();
        return cell?.WorksheetColumn().ColumnLetter();
    }
    public static string GetColumnLetterByHeader(ExcelWorksheet sheet, string columnHeader)
    {
        var columnNumber = GetColumnNumberByHeader(sheet, columnHeader);

        return columnNumber == null ? null
            : ExcelCellAddress.GetColumnLetter(columnNumber.Value);
    }
    #endregion
    #region GetColumnNumberByHeader
    public static int? GetColumnNumberByHeader(IXLWorksheet sheet, string columnHeader)
    {
        var cell = sheet.FirstRowUsed().CellsUsed(c => c.GetString() == columnHeader).FirstOrDefault();
        return cell?.WorksheetColumn().ColumnNumber();
    }
    public static int? GetColumnNumberByHeader(ExcelWorksheet sheet, string columnHeader)
    {
        var startRowNumber = sheet.Dimension.Start.Row;
        var startColumnNumber = sheet.Dimension.Start.Column;
        var endColumnNumber = sheet.Dimension.End.Column;

        var columnNumber = Enumerable.Range(startColumnNumber, endColumnNumber)
            .FirstOrDefault(e => sheet.Cells[startRowNumber, e].GetValue<string>() == columnHeader);

        return columnNumber;
    }
    #endregion

    #region GetColumnHeaders
    public static string?[] GetColumnHeaders(IXLWorksheet sheet)
    {
        var headers = sheet.FirstRowUsed().CellsUsed().Select(c => c.GetString())
            .GetValueOrEmpty()
            .ToArray();
        return headers;
    }
    public static string?[] GetColumnHeaders(ExcelWorksheet sheet)
    {
        var startRowNumber = sheet.Dimension.Start.Row;
        var startColumnNumber = sheet.Dimension.Start.Column;
        var endColumnNumber = sheet.Dimension.End.Column;

        var headers = Enumerable.Range(startColumnNumber, endColumnNumber)
            .Select(e => sheet.Cells[startRowNumber, e].GetValue<string>())
            .GetValueOrEmpty()
            .ToArray();
        return headers;
    }
    #endregion

    #region ReadColumnHeaders
    public static IEnumerable<ExcelColumn> ReadColumnHeaders(IXLWorksheet sheet)
    {
        var columns = sheet.ColumnsUsed().ToArray()
            .Select(column => new ExcelColumn { Letter = column.ColumnLetter(), Number = column.ColumnNumber(), Header = column.Cell(1).GetString() });
        return columns;
    }
    public static IEnumerable<ExcelColumn> ReadColumnHeaders(ExcelWorksheet sheet)
    {
        var startRowNumber = sheet.Dimension.Start.Row;
        var startColumnNumber = sheet.Dimension.Start.Column;
        var endColumnNumber = sheet.Dimension.End.Column;

        var columns = Enumerable.Range(startColumnNumber, endColumnNumber)
            .Select(i => new ExcelColumn { Letter = ExcelCellAddress.GetColumnLetter(i), Number = i, Header = sheet.Cells[startRowNumber, i].GetValue<string>() });
        return columns;
    }
    #endregion
    #region WriteColumnHeaders
    public static void WriteColumnHeaders<T>(IXLWorksheet sheet, IEnumerable<DataField<T>> fields)
    {
        foreach (var field in fields)
        {
            sheet.Cell(1, field.Number).Value = field.Header;
            sheet.Cell(1, field.Number).Style.Font.Bold = true;
        }
    }
    public static void WriteColumnHeaders<T>(ExcelWorksheet sheet, IEnumerable<DataField<T>> fields)
    {
        foreach (var field in fields)
        {
            sheet.Cells[1, field.Number].Value = field.Header;
            sheet.Cells[1, field.Number].Style.Font.Bold = true;
        }
    }
    #endregion
}
