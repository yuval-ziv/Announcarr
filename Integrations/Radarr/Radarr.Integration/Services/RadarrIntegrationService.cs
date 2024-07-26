using Announcarr.Clients.Radarr.Client;
using Announcarr.Clients.Radarr.Responses;
using Announcarr.Integrations.Abstractions.Interfaces;
using Announcarr.Integrations.Abstractions.Responses;
using Announcarr.Integrations.Radarr.Integration.Configurations;
using Announcarr.Integrations.Radarr.Integration.Contracts;
using Announcarr.Utils.Extensions.DateTime;

namespace Announcarr.Integrations.Radarr.Integration.Services;

public class RadarrIntegrationService : IIntegrationService
{
    private readonly RadarrIntegrationConfiguration _configuration;

    public RadarrIntegrationService(RadarrIntegrationConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsEnabled => _configuration.IsEnabled;

    public string GetName() => _configuration.Name ?? "Radarr";
    public bool IsGetCalendarEnabled => _configuration.IsGetCalendarEnabled;

    public async Task<CalendarResponse> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (!IsGetCalendarEnabled)
        {
            return new CalendarResponse();
        }

        using var radarrApiClient = new RadarrApiClient(_configuration.Url, _configuration.ApiKey!, _configuration.IgnoreCertificateValidation);

        return new CalendarResponse
        {
            CalendarItems = await GetCalendarItems(radarrApiClient, from, to, false, cancellationToken),
        };
    }

    public bool IsGetRecentlyAddedEnabled => _configuration.IsGetRecentlyAddedEnabled;

    public async Task<RecentlyAddedResponse> GetRecentlyAddedAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (!IsGetRecentlyAddedEnabled)
        {
            return new RecentlyAddedResponse();
        }

        using IRadarrApiClient radarrApiClient = new RadarrApiClient(_configuration.Url, _configuration.ApiKey!, _configuration.IgnoreCertificateValidation);

        return new RecentlyAddedResponse
        {
            NewlyMonitoredItems = await GetNewlyMonitoredItemsAsync(radarrApiClient, from, to, cancellationToken),
            NewItems = await GetCalendarItems(radarrApiClient, from, to, true, cancellationToken),
        };
    }

    private async Task<List<BaseCalendarItem>> GetCalendarItems(IRadarrApiClient radarrApiClient, DateTimeOffset from, DateTimeOffset to, bool onlyMoviesWithFile, CancellationToken cancellationToken)
    {
        List<MovieResource> episodeResources = await radarrApiClient.GetCalendarAsync(from, to, cancellationToken: cancellationToken);

        return episodeResources.Where(movie => !onlyMoviesWithFile || (movie.HasFile ?? false)).Select(movie => ToRadarrCalendarItem(movie, from, to)).Cast<BaseCalendarItem>().ToList();
    }

    private async Task<List<NewlyMonitoredItem>> GetNewlyMonitoredItemsAsync(IRadarrApiClient radarrApiClient, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        List<MovieResource> movieResources = await radarrApiClient.GetMoviesAsync(cancellationToken: cancellationToken);

        return movieResources.Where(movie => movie.Added?.Between(from.DateTime, to) ?? false).Select(movie => ToNewlyMonitoredMovie(movie, from, to)).Cast<NewlyMonitoredItem>().ToList();
    }

    private RadarrCalendarItem ToRadarrCalendarItem(MovieResource movie, DateTimeOffset from, DateTimeOffset to)
    {
        (DateTimeOffset? relevantDate, ReleaseDateType relevantDateType) = GetRelevantDate(movie, from, to);

        return new RadarrCalendarItem
        {
            CalendarItemSource = GetName(),
            ReleaseDate = relevantDate,
            ReleaseDateType = relevantDateType,
            ThumbnailUrl = GetThumbnailUrl(movie),
            MovieName = movie.Title,
        };
    }

    private static (DateTimeOffset? relevantDate, ReleaseDateType relevantDateType) GetRelevantDate(MovieResource movie, DateTimeOffset from, DateTimeOffset to)
    {
        if (movie.PhysicalRelease > from && movie.PhysicalRelease < to)
        {
            return (movie.PhysicalRelease, ReleaseDateType.PhysicalRelease);
        }

        if (movie.DigitalRelease > from && movie.DigitalRelease < to)
        {
            return (movie.DigitalRelease, ReleaseDateType.DigitalRelease);
        }

        return (movie.InCinemas, ReleaseDateType.InCinemas);
    }

    private NewlyMonitoredMovie ToNewlyMonitoredMovie(MovieResource movie, DateTimeOffset from, DateTimeOffset to)
    {
        (DateTimeOffset? relevantDate, ReleaseDateType relevantDateType) = GetRelevantDate(movie, from, to);

        return new NewlyMonitoredMovie
        {
            CalendarItemSource = GetName(),
            StartedMonitoring = movie.Added,
            ThumbnailUrl = GetThumbnailUrl(movie),
            MovieName = movie.Title,
            ReleaseDate = relevantDate,
            ReleaseDateType = relevantDateType,
            IsAvailable = movie.HasFile ?? false,
        };
    }

    private static string? GetThumbnailUrl(MovieResource? movie)
    {
        return movie?.Images?.FirstOrDefault(cover => cover.CoverType == MediaCoverTypes.Poster)?.RemoteUrl;
    }

    public Task GetNewAnnouncementAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}