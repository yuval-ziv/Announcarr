using Announcarr.Abstractions.Contracts.Contracts;
using Announcarr.Clients.Radarr.Client;
using Announcarr.Clients.Radarr.Responses;
using Announcarr.Integrations.Abstractions.AbstractImplementations;
using Announcarr.Integrations.Radarr.Integration.Configurations;
using Announcarr.Integrations.Radarr.Integration.Contracts;
using Announcarr.Utils.Extensions.DateTime;

namespace Announcarr.Integrations.Radarr.Integration.Services;

public class RadarrIntegrationService : BaseIntegrationService
{
    private readonly RadarrIntegrationConfiguration _configuration;

    public RadarrIntegrationService(RadarrIntegrationConfiguration configuration)
    {
        _configuration = configuration;
    }

    public override bool IsEnabled => _configuration.IsEnabled;

    public override string Name => _configuration.Name ?? "Radarr";
    public override bool IsGetCalendarEnabled => _configuration.IsEnabledByAnnouncementType(AnnouncementType.Calendar);

    public override bool IsGetRecentlyAddedEnabled => _configuration.IsEnabledByAnnouncementType(AnnouncementType.RecentlyAdded);

    protected override async Task<CalendarContract> GetCalendarLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using var radarrApiClient = new RadarrApiClient(_configuration.Url, _configuration.ApiKey!, _configuration.IgnoreCertificateValidation);

        return new CalendarContract
        {
            CalendarItems = await GetCalendarItems(radarrApiClient, from, to, false, cancellationToken),
        };
    }

    protected override async Task<RecentlyAddedContract> GetRecentlyAddedLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using IRadarrApiClient radarrApiClient = new RadarrApiClient(_configuration.Url, _configuration.ApiKey!, _configuration.IgnoreCertificateValidation);

        return new RecentlyAddedContract
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
            CalendarItemSource = Name,
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
            CalendarItemSource = Name,
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
}