using System.Collections.Generic;

namespace Common
{
    public static class ListExtensions
    {
        public static T? ReadByIndex<T>(this IList<T>? list, int index)
            where T : class
        {
            return list?.Count > index ? list[index] : null;
        }
    }
}