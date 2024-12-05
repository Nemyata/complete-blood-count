using System.Collections.Generic;
using System.Dynamic;

namespace Common
{
    public class OptionalToolkit
    {
        public static dynamic CleanArguments(dynamic dirty)
        {
            dirty = (IDictionary<string, object>)ObjectExtensions.ToExpando(dirty);

            dynamic filter = new ExpandoObject();
            var dic = (IDictionary<string, object>)filter;

            foreach (KeyValuePair<string, object> kvp in dirty)
            {
                var key = kvp.Key;
                var value = (IOptional)kvp.Value;

                if (!value.HasValue)
                    continue;

                dic.Add(new KeyValuePair<string, object>(key, value.Value));
            }

            return filter;
        }
    }
}