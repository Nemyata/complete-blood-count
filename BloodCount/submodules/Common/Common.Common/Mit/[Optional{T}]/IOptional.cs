namespace Common
{
    /// <summary>
    /// Базовый интерфейс для <see cref="Optional{T}"/>,
    /// предоставляющий возможность получения значения свойств <see cref="Optional{T}.HasValue"/> и <see cref="Optional{T}.Value"/>,
    /// не зная типа T, использованного в конкретном обобщённом типе <see cref="Optional{T}"/>.
    /// </summary>
    public interface IOptional
    {
        bool HasValue { get; }
        object Value { get; }
    }
}