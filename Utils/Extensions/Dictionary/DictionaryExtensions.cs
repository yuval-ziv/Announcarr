namespace Announcer.Utils.Extensions.Dictionary;

public static class DictionaryExtensions
{
    public static IEnumerable<TResult> Select<TSourceKey, TSourceValue, TResult>(this IDictionary<TSourceKey, TSourceValue> dictionary, Func<TSourceKey, TSourceValue, TResult> selector)
        where TSourceKey : notnull
    {
        return dictionary.Select(kvp => selector(kvp.Key, kvp.Value));
    }
}