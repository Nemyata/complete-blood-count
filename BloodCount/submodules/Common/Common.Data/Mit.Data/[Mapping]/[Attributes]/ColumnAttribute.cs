using System;
using System.Runtime.CompilerServices;

namespace Common.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
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
        public string? ExcelNumberFormat { get; set; }
        /// <summary>
        /// Заголовок - текстовое значение первой ячейки в колонке Excel файла.
        /// </summary>
        public string? Header { get; set; }
        /// <summary>
        /// Значение по умолчанию. Присваивается в случае, если ячейке хранится пустая строка.
        /// </summary>
        public object? DefaultValue { get; set; }
        /// <summary>
        /// Путь файла класса, к свойствам которого применяется атрибут <see cref="ColumnAttribute"/>.
        /// </summary>
        /// <remarks>Необходим для вычисления номеров колонок для свойств объектов <code>partial</code> класса,
        /// состоящего из нескольких файлов.</remarks>
        internal string? FilePath { get; set; }
        /// <summary>
        /// Номер строки, в которой объявляется атрибут <see cref="ColumnAttribute"/>.
        /// </summary>
        /// <remarks>Необходим для сортировки свойств объекта по порядку объявления атрибутов <see cref="ColumnAttribute"/>,
        /// а потому по порядку объявления самих свойств.</remarks>
        internal int? LineNumber { get; set; }

        public ColumnAttribute([CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1)
        {
            FilePath = filePath;
            LineNumber = lineNumber;
        }
        public ColumnAttribute(string letter, string? format = null)
        {
            Letter = letter.ToUpperInvariant();
            Format = format;
        }
        public ColumnAttribute(int number, string? format = null)
        {
            Number = number;
            Format = format;
        }
        public ColumnAttribute(string format, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1)
        {
            Format = format;
            FilePath = filePath;
            LineNumber = lineNumber;
        }
    }
}
