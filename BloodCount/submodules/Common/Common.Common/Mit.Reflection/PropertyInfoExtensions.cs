using System;
using System.Reflection;

namespace Common.Reflection
{
    /// <summary>
    /// Extension-класс для создания Lambda-выражений, предоставляющих быстрый типизированный доступ к getter/setter объектов <see cref="PropertyInfo"/>.
    /// </summary>
    /// <remarks>
    /// Оригинал заимствован отсюда:
    /// <see href="https://stackoverflow.com/a/17669142/3905944">c# - Is it possible to speed this method up? - Stack Overflow</see>
    /// </remarks>
    public static class PropertyInfoExtensions
    {
        public static Func<T, TReturn> BuildTypedGetter<T, TReturn>(this PropertyInfo propertyInfo)
        {
            var method = propertyInfo.GetGetMethod();
            if (method == null)
                throw new ApplicationException($"Given {nameof(propertyInfo)} has no getter.");

            var get = (Func<T, TReturn>)Delegate.CreateDelegate(typeof(Func<T, TReturn>), method);
            return get;
        }
        public static Action<T, TProperty> BuildTypedSetter<T, TProperty>(this PropertyInfo propertyInfo)
        {
            var method = propertyInfo.GetSetMethod();
            if (method == null)
                throw new ApplicationException($"Given {nameof(propertyInfo)} has no setter.");

            var set = (Action<T, TProperty>)Delegate.CreateDelegate(typeof(Action<T, TProperty>), method);
            return set;
        }
    }
}
