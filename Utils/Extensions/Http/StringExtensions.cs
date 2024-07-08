using System.Web;
using Announcer.Utils.Extensions.Dictionary;

namespace Announcer.Utils.Extensions.Http;

public static class StringExtensions
{
    public static string WithQueryParameters(this string api, IDictionary<string, string> queryParameters)
    {
        return api + "?" + string.Join("&", queryParameters.Select((key, value) => key + "=" + HttpUtility.UrlEncode(value)));
    }
}