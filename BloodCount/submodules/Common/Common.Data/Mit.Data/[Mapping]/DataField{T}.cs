using System;
using System.Linq.Expressions;
using System.Reflection;

using Common.Reflection;
using MemberInfoExtensions = Common.Reflection.MemberInfoExtensions;

namespace Common.Data;

public class DataField<T>
{
    private string? excelNumberFormat;

    public DataField(Type propertyType)
    {
        PropertyType = propertyType;
    }
    public DataField(string propertyName, ColumnAttribute? attr = null)
        : this(typeof(T).GetProperty(propertyName)!, attr)
    {
    }
    public DataField(Expression<Func<T, object>> propertyExpression, ColumnAttribute? attr = null)
        : this(MemberInfoExtensions.GetPropertyInfo(propertyExpression)!, attr)
    {
    }
    public DataField(PropertyInfo prop, ColumnAttribute? attr = null)
    {
        PropertyType = prop.PropertyType;
        if (prop.GetGetMethod() != null)
            Getter = prop.BuildUntypedGetter<T>();
        if (prop.GetSetMethod() != null)
            Setter = prop.BuildUntypedSetter<T>();

        if (attr == null)
            return;

        Letter = attr.Letter;
        Number = attr.Number;
        Format = attr.Format;
        ExcelNumberFormat = attr.ExcelNumberFormat;
        Header = attr.Header ?? prop.Name;
        DefaultValue = attr.DefaultValue;
    }

    /// <summary>
    /// Буквенный код колонки в Excel файле.
    /// </summary>
    public string? Letter { get; set; }
    /// <summary>
    /// Числовой код колонки в Excel файле.
    /// </summary>
    public int Number
    {
        get => ExcelToolkit.GetNumber(Letter);
        set => Letter = ExcelToolkit.GetLetter(value);
    }
    /// <summary>
    /// Числовой формат данных, используемый для форматирования ячейки в Excel файле.
    /// </summary>
    public string? Format { get; set; }
    /// <summary>
    /// Числовой формат форматирования ячеек в Excel таблицах.
    /// Используется для настройки представления ячеек Excel, например объектов DateTime, float.
    /// </summary>
    public string? ExcelNumberFormat
    {
        get => string.IsNullOrEmpty(excelNumberFormat) && PropertyType.GetUnderlyingType() == typeof(DateTime)
            ? "dd.mm.yyyy HH:mm"
            : excelNumberFormat;
        set => excelNumberFormat = value;
    }
    /// <summary>
    /// Заголовок - текстовое значение первой ячейки в колонке Excel файла.
    /// </summary>
    public string? Header { get; set; }
    /// <summary>
    /// Значение по умолчанию. Присваивается в случае, если исходное значение “пустое”.
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Lambda-выражение для быстрого получения значения из заданного свойства entity типа <typeparamref name="T"/>.<br />
    /// Также используется для генерации значения ячейки без привязки к одному конкретному свойству:
    /// при помощи конвертации значения некоторого свойства, составления нового значения из нескольких независимых свойств объекта,
    /// запись константы и др. значений (например, <see cref="DateTime.Now"/>).
    /// </summary>
    public Func<T, object?>? Getter { get; set; }
    /// <summary>
    /// Lambda-выражение для быстрого присвоения значения заданному свойству entity типа <typeparamref name="T"/>.<br />
    /// Также используется для разбора (парсинга) значения ячейки и последующего присваивания полученных данных одной/сразу нескольким свойствам объекта.
    /// </summary>
    public Action<T, object?>? Setter { get; set; }

    /// <summary>
    /// Тип целевого свойства объекта.
    /// </summary>
    internal Type PropertyType { get; set; }
}
