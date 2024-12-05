using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Common.Collections.Specialized;

public static class NameValueCollectionExtensions
{
    /// <see href="https://stackoverflow.com/questions/391023/make-namevaluecollection-accessible-to-linq-query">
    /// .net - Make NameValueCollection accessible to LINQ Query - Stack Overflow</see></remarks>
    public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(this NameValueCollection collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        return collection.Cast<string>()
            .Select(key => new KeyValuePair<string, string>(key, collection[key]!));
    }
}
