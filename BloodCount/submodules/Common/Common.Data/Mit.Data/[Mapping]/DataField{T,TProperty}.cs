using System;
using System.Linq.Expressions;
using System.Reflection;

using Common.Reflection;
using MemberInfoExtensions = Common.Reflection.MemberInfoExtensions;

namespace Common.Data;

public class DataField<T, TProperty> : DataField<T>
{
    private Func<T, TProperty>? _typedGetter;
    private Action<T, TProperty>? _typedSetter;

    public DataField()
        : base(typeof(TProperty))
    {
    }
    public DataField(Expression<Func<T, TProperty>> propertyExpression, ColumnAttribute? attr = null)
        : this(MemberInfoExtensions.GetPropertyInfo(propertyExpression)!, attr)
    {
    }
    public DataField(PropertyInfo prop, ColumnAttribute? attr = null)
        : base(typeof(TProperty))
    {
        if (prop.GetGetMethod() != null)
        {
            Getter = prop.BuildUntypedGetter<T>();
            _typedGetter = prop.BuildTypedGetter<T, TProperty>();
        }
        if (prop.GetSetMethod() != null)
        {
            Setter = prop.BuildUntypedSetter<T>();
            _typedSetter = prop.BuildTypedSetter<T, TProperty>();
        }

        if (attr == null)
            return;

        Letter = attr.Letter;
        Number = attr.Number;
        Format = attr.Format;
        ExcelNumberFormat = attr.ExcelNumberFormat;
        Header = attr.Header;
        DefaultValue = (TProperty)attr.DefaultValue!;
    }

    /// <summary>
    /// Значение по умолчанию. Присваивается в случае, если исходное значение “пустое”.
    /// </summary>
    public TProperty TypedDefaultValue
    {
        get { return (TProperty)DefaultValue!; }
        set { DefaultValue = value; }
    }

    /// <summary>
    /// Lambda-выражение для быстрого получения значения из заданного свойства entity типа <typeparamref name="T"/>.<br />
    /// Также используется для генерации значения ячейки без привязки к одному конкретному свойству:
    /// при помощи конвертации значения некоторого свойства, составления нового значения из нескольких независимых свойств объекта,
    /// запись константы и др. значений (например, <see cref="DateTime.Now"/>).
    /// </summary>
    public Func<T, TProperty>? TypedGetter
    {
        get { return _typedGetter; }
        set
        {
            _typedGetter = value;
            Getter = _typedGetter == null ? null
                : e => _typedGetter(e);
        }
    }

    /// <summary>
    /// Lambda-выражение для быстрого присвоения значения заданному свойству entity типа <typeparamref name="T"/>.<br />
    /// Также используется для разбора (парсинга) значения ячейки и последующего присваивания полученных данных одной/сразу нескольким свойствам объекта.
    /// </summary>
    public Action<T, TProperty>? TypedSetter
    {
        get { return _typedSetter; }
        set
        {
            _typedSetter = value;
            Setter = _typedSetter == null ? null
                : (e, v) => _typedSetter(e, (TProperty)v!);
        }
    }
}
