using Announcarr.Clients.Sonarr.Responses;

namespace Announcarr.Clients.Sonarr.Client;

public interface ISonarrApiClient : IDisposable
{
    Task<List<EpisodeResource>> GetCalendarAsync(DateTimeOffset start, DateTimeOffset end, bool unmonitored = false, bool includeSeries = false, bool includeEpisodeFile = false,
        bool includeEpisodesImages = false, string tags = "", CancellationToken cancellationToken = default);
}