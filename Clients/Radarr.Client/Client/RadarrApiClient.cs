using System.Runtime.CompilerServices;
using Announcer.Clients.Radarr.Responses;
using Announcer.Utils.Extensions.Http;
using Newtonsoft.Json;

namespace Announcer.Clients.Radarr.Client;

public class RadarrApiClient : IRadarrApiClient
{
    private const string? RequestForComments3339Section5Point6DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fffK";
    private readonly HttpClient _httpClient;

    public RadarrApiClient(string baseAddress, string apiKey, bool ignoreCertificateValidation)
    {
        var httpClientHandler = new HttpClientHandler();
        if (ignoreCertificateValidation)
        {
            httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            httpClientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        }

        _httpClient = new HttpClient(httpClientHandler)
        {
            BaseAddress = new Uri(baseAddress),
            DefaultRequestHeaders = { { "X-Api-Key", apiKey } },
        };
    }

    public async Task<List<MovieResource>> GetCalendarAsync(DateTimeOffset start, DateTimeOffset end, bool unmonitored = false, string tags = "", CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/v3/calendar".WithQueryParameters(new Dictionary<string, string>
        {
            { "start", start.ToString(RequestForComments3339Section5Point6DateTimeFormat) },
            { "end", end.ToString(RequestForComments3339Section5Point6DateTimeFormat) },
            { "unmonitored", unmonitored.ToString() },
            { "tags", tags },
        }), cancellationToken);

        ThrowIfNotSuccessStatusCode(httpResponseMessage);

        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<List<MovieResource>>(responseContent) ?? [];
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    private static void ThrowIfNotSuccessStatusCode(HttpResponseMessage httpResponseMessage, [CallerMemberName] string memberName = "")
    {
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            throw new UnauthorizedAccessException($"Sonarr returned {httpResponseMessage.StatusCode} for {memberName}");
        }
    }
}