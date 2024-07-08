using Announcer.Clients.Radarr.Responses;

namespace Announcer.Clients.Radarr.Client;

public interface IRadarrApiClient : IDisposable
{
    Task<List<MovieResource>> GetCalendarAsync(DateTimeOffset start, DateTimeOffset end, bool unmonitored = false, string tags = "", CancellationToken cancellationToken = default);
}