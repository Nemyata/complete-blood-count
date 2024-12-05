using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;
using OfficeOpenXml;

using Common.Linq;

namespace Common.Data;

public class DataMapper
{
    #region Map<T>(DataRow, T)
    public static T Map<T>(DataRow row, T entity, IEnumerable<DataField<T>>? mask = null, Action<string> logger = null)
    {
        var fields = RefreshMask(mask, row.Table);

        MapInternal(row, entity, fields, logger);
        return entity;
    }
    public static IEnumerable<T> Map<T>(DataTable table, IEnumerable<DataField<T>>? mask = null, Action<string> logger = null)
        where T : new()
    {
        var fields = RefreshMask(mask, table);

        var targetList = new List<T>();
        foreach (DataRow row in table.Rows)
        {
            var entity = new T();
            MapInternal(row, entity, fields, logger);
            targetList.Add(entity);
        }

        return targetList;
    }
    private static void MapInternal<T>(DataRow row, T entity, IEnumerable<DataField<T>> fields, Action<string> logger)
    {
        var rowNumber = row.Table.Rows.IndexOf(row) + 1;
        ProcessTable(rowNumber,
            columnIndex =>
            {
                var cellValue = row[columnIndex];
                var cellString = cellValue.ToString();

                return (cellValue, cellString);
            },
            (columnIndex, field) =>
            {
                var type = field.PropertyType;

                var cellValue = row[columnIndex];

                var s = TypeToolkit.TryGetValue(cellValue, type, out var value);
                return (s, value);
            },
            entity, fields, logger
        );
    }

    public static void Map<T>(T entity, DataRow row, IEnumerable<DataField<T>>? mask = null, Action<string> logger = null)
    {
        var fields = RefreshMask(mask, row.Table, true);

        MapInternal(entity, row, fields, logger);
    }
    public static void Map<T>(IEnumerable<T> entities, DataTable table, IEnumerable<DataField<T>>? mask = null, Action<string> logger = null)
    {
        var fields = RefreshMask(mask, table, true);

        foreach (var entity in entities)
        {
            var row = table.NewRow();

            MapInternal(entity, row, fields, logger);
        }
    }
    private static void MapInternal<T>(T entity, DataRow row, IEnumerable<DataField<T>> fields, Action<string> logger)
    {
        ProcessEntity(fields, field =>
        {
            var name = field.Header;

            if (string.IsNullOrEmpty(name))
                return;

            var value = field.Getter!(entity);
            row[name] = (value == DBNull.Value || value == null) ? DBNull.Value : value;
        }, logger);
    }
    #endregion

    #region Map<T>(IXLRow, T)
    public static T Map<T>(IXLRow row, T entity, IEnumerable<DataField<T>>? mask = null, Action<string> logger = null)
    {
        var fields = RefreshMask(mask, row.Worksheet);

        MapInternal(row, entity, fields, logger);
        return entity;
    }
    public static IEnumerable<T> Map<T>(IXLWorksheet sheet, bool header = true, IEnumerable<DataField<T>>? mask = null, Action<string> logger = null)
        where T : new()
    {
        var fields = RefreshMask(mask, sheet);

        var rows = ExcelToolkit.GetRows(sheet).ToArray();
        if (header && rows.Length > 0)
            rows = rows.Skip(1).ToArray();

        var targetList = new List<T>();
        foreach (IXLRow row in rows)
        {
            var entity = new T();
            MapInternal(row, entity, fields, logger);
            targetList.Add(entity);
        }

        return targetList;
    }
    private static void MapInternal<T>(IXLRow row, T entity, IEnumerable<DataField<T>> fields, Action<string> logger)
    {
        var rowNumber = row.RowNumber();
        ProcessTable(rowNumber,
            columnIndex =>
            {
                var cell = row.Cell(columnIndex + 1);
                var cellValue = cell.Value;
                var cellString = cell.GetString();

                return (cellValue, cellString);
            },
            (columnIndex, field) =>
            {
                var type = field.PropertyType;

                var cell = row.Cell(columnIndex + 1);
                var cellValue = TypeToolkitExt.NormalizeValue(cell.Value, type);
                var cellString = cell.GetString();

                var s = TypeToolkit.TryGetValue(cellValue, type, out var value)
                    || TypeToolkitExt.TryGetValue(cellString, field, out value);
                if (s && type.IsNullable(out _))
                    value = value!.CastTo(type); // Костыль на замену типа на underlyingType. Необходим только для IXLRow
                return (s, value);
            },
            entity, fields, logger
        );
    }

    public static void Map<T>(T entity, IXLRow row, IEnumerable<DataField<T>>? mask = null, Action<string> logger = null)
    {
        var fields = RefreshMask(mask, row.Worksheet, true);
        MapInternal(entity, row, fields, logger);
    }
    public static void Map<T>(IEnumerable<T> entities, IXLWorksheet sheet, IEnumerable<DataField<T>>? mask = null, bool header = true, Action<string> logger = null)
    {
        var fields = RefreshMask(mask, sheet, true);

        var rowNumber = 1;
        if (header)
        {
            ExcelToolkit.WriteColumnHeaders(sheet, fields);
            rowNumber++;
        }

        foreach (var entity in entities)
        {
            MapInternal(entity, sheet.Row(rowNumber), fields, logger);
            rowNumber++;
        }
    }
    private static void MapInternal<T>(T entity, IXLRow row, IEnumerable<DataField<T>> fields, Action<string> logger)
    {
        ProcessEntity(fields, field =>
        {
            var letter = field.Letter;
            var format = field.Format;

            var value = field.Getter!(entity);
            if (value != null && !string.IsNullOrEmpty(format))
                value = TypeToolkit.ToString(value, format);
            row.Cell(letter).SetValue(value);

            // Присвоение значение через свойство 'row.Cell(letter).Value = value;' запускает парсинг строки и
            // приведение ее к подходящему типу. например, к дате, которая затем записывается в ячейку в ином формате.
            // Что, очевидно, крайне нежелательно при импорте подобного файла прочими автоматизированными системами.
        }, logger);
    }
    #endregion

    #region Map<TSource, TDestination>(TSource, TDestination)
    public static void Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProps = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        MapInternal(source, destination, sourceProps, destinationProps);
    }
    public static IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> entities)
        where TDestination : new()
    {
        var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProps = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var targetList = new List<TDestination>();
        foreach (var entity in entities)
        {
            var destination = new TDestination();
            MapInternal(entity, destination, sourceProps, destinationProps);
            targetList.Add(destination);
        }

        return targetList;
    }
    private static void MapInternal<TSource, TDestination>(TSource source, TDestination destination, PropertyInfo[] sourceProps, PropertyInfo[] destinationProps)
    {
        var joined = sourceProps.Join(destinationProps,
            e => e.Name, e => e.Name, (e1, e2) => new { source = e1, destination = e2 });

        foreach (var conn in joined)
        {
            if (conn.source == null || conn.destination == null)
                continue;

            var value = conn.source.GetValue(source);
            conn.destination.SetValue(destination, value);
        }
    }
    #endregion

    #region Map<T>(ExcelWorksheet, T)
    public static T Map<T>(ExcelWorksheet sheet, int rowNumber, T entity, IEnumerable<DataField<T>>? mask = null, Action<string> logger = null)
    {
        var fields = RefreshMask(mask, sheet);

        MapInternal(sheet, rowNumber, entity, fields, logger);
        return entity;
    }
    public static IEnumerable<T> Map<T>(ExcelWorksheet sheet, IEnumerable<DataField<T>>? mask = null, bool header = true, Action<string> logger = null)
        where T : new()
    {
        var fields = RefreshMask(mask, sheet);

        var rows = ExcelToolkit.GetRows(sheet);
        if (header)
            rows = rows.Skip(1);

        var targetList = new List<T>();
        foreach (var row in rows)
        {
            var entity = new T();
            MapInternal(sheet, row, entity, fields, logger);
            targetList.Add(entity);
        }

        return targetList;
    }
    private static void MapInternal<T>(ExcelWorksheet sheet, int rowNumber, T entity, IEnumerable<DataField<T>> fields, Action<string> logger)
    {
        ProcessTable(rowNumber,
            columnIndex =>
            {
                var cell = sheet.Cells[rowNumber, columnIndex + 1];
                var cellValue = cell.Value;
                var cellString = cell.GetValue<string>();

                return (cellValue, cellString);
            },
            (columnIndex, field) =>
            {
                var type = field.PropertyType;

                var cell = sheet.Cells[rowNumber, columnIndex + 1];
                var cellValue = TypeToolkitExt.NormalizeValue(cell.Value, type);
                var cellString = cell.GetValue<string>();

                var s = TypeToolkit.TryGetValue(cellValue, type, out var value)
                    || TypeToolkitExt.TryGetValue(cellString, field, out value);
                return (s, value);
            },
            entity, fields, logger
        );
    }

    public static void Map<T>(T entity, ExcelWorksheet sheet, int rowNumber, IEnumerable<DataField<T>>? mask = null, Action<string> logger = null)
    {
        var fields = RefreshMask(mask, sheet, true);
        MapInternal(entity, sheet, rowNumber, fields, logger);
    }
    public static void Map<T>(IEnumerable<T> entities, ExcelWorksheet sheet, IEnumerable<DataField<T>>? mask = null, bool header = true, Action<string> logger = null)
    {
        var fields = RefreshMask(mask, sheet, true);

        var rowNumber = 1;
        if (header)
        {
            ExcelToolkit.WriteColumnHeaders(sheet, fields);
            rowNumber++;
        }

        foreach (var entity in entities)
        {
            MapInternal(entity, sheet, rowNumber, fields, logger);
            rowNumber++;
        }
    }
    private static void MapInternal<T>(T entity, ExcelWorksheet sheet, int rowNumber, IEnumerable<DataField<T>> fields, Action<string> logger)
    {
        ProcessEntity(fields, field =>
        {
            var number = field.Number;
            var format = field.Format;
            var excelFormat = field.ExcelNumberFormat;

            var value = field.Getter!(entity);

            sheet.Cells[rowNumber, number].Value = (value == null || string.IsNullOrEmpty(format)) ? value
                : TypeToolkit.ToString(value, format);

            if (!string.IsNullOrEmpty(excelFormat))
                sheet.Cells[rowNumber, number].Style.Numberformat.Format = excelFormat;
        }, logger);
    }
    #endregion


    #region Core: processing
    private static void ProcessTable<T>(int rowNumber,
        Func<int, (object? cellValue, string? cellString)> cellData,
        Func<int, DataField<T>, (bool success, object? value)> getCellValue,
        T entity, IEnumerable<DataField<T>> fields, Action<string> logger)
    {
        foreach (var field in fields)
        {
            if (field.Setter == null)
                continue;

            var index = field.Number - 1;
            if (index < 0)
            {
                logger?.Invoke($"Невозможно определить номер колонки (ожидаемый заголовок: “{field.Header}”).");
                continue;
            }

            var type = field.PropertyType;
            var (cellValue, cellString) = cellData(index);

            Exception? thrownEx = null;
            try
            {
                if (type.IsNullable(out var underlyingType))
                {
                    if (cellValue == null || string.IsNullOrEmpty(cellString))
                    {
                        var result = field.DefaultValue ?? underlyingType.DefaultValue();
                        field.Setter(entity, result);
                        continue;
                    }

                    type = underlyingType;
                }

                var value = getCellValue(index, field);
                if (value.success)
                {
                    field.Setter(entity, value.value);
                    continue;
                }
            }
            catch (Exception ex)
            {
                thrownEx = ex;
            }

            if (logger == null)
                continue;

            var message = $"Номер строки: {rowNumber}, ячейка: {ExcelToolkit.GetLetter(index + 1)} ({index + 1}); невозможно привести значение '{cellString}' к типу '{type}'.";
            if (thrownEx != null)
                message += $" Внутренняя ошибка: '{thrownEx.Message}'.";
            logger(message);
        }
    }
    private static void ProcessEntity<T>(IEnumerable<DataField<T>> fields, Action<DataField<T>> handleField, Action<string> logger)
    {
        foreach (var field in fields)
        {
            if (field.Getter == null)
                continue;

            try
            {
                handleField(field);
            }
            catch (Exception ex)
            {
                if (logger == null)
                    throw;
                logger(ex.Message);
            }
        }
    }
    #endregion


    #region RefreshMask
    private static IEnumerable<DataField<T>> RefreshMask<T>(IEnumerable<DataField<T>>? mask, DataTable table, bool write = false)
    {
        var fields = RefreshMask(mask, write ? null : () => DataTableToolkit.GetColumnHeaders(table));
        return fields;
    }
    private static IEnumerable<DataField<T>> RefreshMask<T>(IEnumerable<DataField<T>>? mask, IXLWorksheet sheet, bool write = false)
    {
        var fields = RefreshMask(mask, write ? null : () => ExcelToolkit.GetColumnHeaders(sheet));
        return fields;
    }
    private static IEnumerable<DataField<T>> RefreshMask<T>(IEnumerable<DataField<T>>? mask, ExcelWorksheet sheet, bool write = false)
    {
        var fields = RefreshMask(mask, write ? null : () => ExcelToolkit.GetColumnHeaders(sheet));
        return fields;
    }
    private static IEnumerable<DataField<T>> RefreshMask<T>(IEnumerable<DataField<T>>? mask, Func<string?[]>? getHeaders)
    {
        mask ??= BuildMask<T>();
        mask = BuildMaskNumeration(mask, getHeaders);
        return mask;
    }

    private static IEnumerable<DataField<T>> BuildMask<T>()
    {
        var fields = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(prop => (prop, TypeToolkit.GetAttribute<ColumnAttribute>(prop)))
            .Where(e => e.Item2 != null)
            .OrderBy(e => e.Item2!.FilePath)
            .ThenBy(e => e.Item2!.LineNumber)
            .Select(e => new DataField<T>(e.Item1, e.Item2));
        return fields;
    }
    private static IEnumerable<DataField<T>> BuildMaskNumeration<T>(IEnumerable<DataField<T>> mask, Func<string?[]>? getHeaders = null)
    {
        var fields = mask.ToArray();
        if (fields.All(e => e.Number > 0))
            return fields;

        // Возможно, стоит раскомментировать выброс исключения, но сейчас где-то выгрузка в Excel может работать
        // по случайному стечению обстоятельств, а это обновление её сломает.
        //if (!fields.All(e => e.Number < 1 && string.IsNullOrEmpty(e.Header)) // Номера всех колонок `Number` определяются исходя из порядка переданных `fields`
        //    && !fields.All(e => e.Number > 0 || !string.IsNullOrEmpty(e.Header))) // Номера всех колонок `Number` либо заданы, или у колонки указан `Header`
        //{
        //    // Number/Header указана не у всех fields, что значит, что новое значение Number должно определяться вперемешку:
        //    //  `Header` не пустой – поиском заголовка
        //    //  `Header` пустой – исходя из порядка переданных `fields`.
        //    // Проблема в том, что в общий набор включены `fields`, номера колонок которых определяются из заголовка,
        //    // но сам факт присутствия их в списке изменяет нумерацию всех прочих его элементов. Это бессмысленно
        //    throw new ApplicationException($"Cannot determine {nameof(DataField<T>.Number)} for all given fields.");
        //}

        string?[]? headers = null;
        if (fields.Any(e => e.Number < 1 && !string.IsNullOrEmpty(e.Header)) && getHeaders != null)
            headers = getHeaders();

        foreach (var (field, i) in fields.Indexed())
        {
            if (field.Number > 0)
                continue;

            var index = headers != null && !string.IsNullOrEmpty(field.Header)
                ? Array.IndexOf(headers, field.Header)
                : i;

            field.Number = index + 1;
        }

        return fields;
    }
    #endregion
}
