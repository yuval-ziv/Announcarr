using System.Web;
using Announcarr.Utils.Extensions.Dictionary;

namespace Announcarr.Utils.Extensions.Http;

public static class StringExtensions
{
    public static string WithQueryParameters(this string api, IDictionary<string, string> queryParameters)
    {
        return api + "?" + string.Join("&", queryParameters.Select((key, value) => key + "=" + HttpUtility.UrlEncode(value)));
    }
}