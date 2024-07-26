using System.Web;
using Announcarr.Utils.Extensions.Dictionary;

namespace Announcarr.Utils.Extensions.Http;

public static class StringExtensions
{
    public static string WithQueryParameters(this string api, IDictionary<string, string?> queryParameters, bool skipNullValues = true)
    {
        return api + "?" + string.Join("&", queryParameters.WhereByKeyValuePair((_, value) => skipNullValues || value is not null).SelectByKeyValuePair(UrlEncode));
    }

    private static string UrlEncode(string key, string? value)
    {
        return key + "=" + HttpUtility.UrlEncode(value);
    }
}