using System.Collections.Generic;

namespace Common.Collections.Generic;

public static class DictionaryToolkit
{
    /// <summary>Gets the value associated with the specified key.</summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, if the key is found or <paramref name="key" /> is <see langword="null" />;
    /// otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</returns>
    public static TValue? TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey? key)
        where TKey : struct
    {
        return key == null ? default : dictionary[(TKey)key];
    }
}