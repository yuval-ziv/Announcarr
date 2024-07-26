using Announcarr.Clients.Radarr.Responses;

namespace Announcarr.Clients.Radarr.Client;

public interface IRadarrApiClient : IDisposable
{
    Task<List<MovieResource>> GetCalendarAsync(DateTimeOffset start, DateTimeOffset end, bool unmonitored = false, string tags = "", CancellationToken cancellationToken = default);
    Task<List<MovieResource>> GetMoviesAsync(int? tmdbId = null, bool? excludeLocalCovers = false, int? languageId = null, CancellationToken cancellationToken = default);
}