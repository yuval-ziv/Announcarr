namespace Announcarr.Utils.Extensions.Dictionary;

public static class DictionaryExtensions
{
    public static IEnumerable<TResult> SelectByKeyValuePair<TSourceKey, TSourceValue, TResult>(this IDictionary<TSourceKey, TSourceValue?> dictionary,
        Func<TSourceKey, TSourceValue?, TResult> selector) where TSourceKey : notnull
    {
        return dictionary.Select(kvp => selector(kvp.Key, kvp.Value));
    }

    public static IEnumerable<TResult> SelectByKeyValuePair<TSourceKey, TSourceValue, TResult>(this IEnumerable<KeyValuePair<TSourceKey, TSourceValue?>> dictionary,
        Func<TSourceKey, TSourceValue?, TResult> selector) where TSourceKey : notnull
    {
        return dictionary.Select(kvp => selector(kvp.Key, kvp.Value));
    }

    public static IEnumerable<KeyValuePair<TSourceKey, TSourceValue?>> WhereByKeyValuePair<TSourceKey, TSourceValue>(this IDictionary<TSourceKey, TSourceValue?> dictionary,
        Func<TSourceKey, TSourceValue?, bool> selector) where TSourceKey : notnull
    {
        return dictionary.Where(kvp => selector(kvp.Key, kvp.Value));
    }

    public static IEnumerable<KeyValuePair<TSourceKey, TSourceValue?>> WhereByKeyValuePair<TSourceKey, TSourceValue>(this IEnumerable<KeyValuePair<TSourceKey, TSourceValue?>> dictionary,
        Func<TSourceKey, TSourceValue?, bool> selector) where TSourceKey : notnull
    {
        return dictionary.Where(kvp => selector(kvp.Key, kvp.Value));
    }
}