using Announcarr.Abstractions.Contracts;
using Announcarr.Clients.Radarr.Client;
using Announcarr.Clients.Radarr.Responses;
using Announcarr.Integrations.Abstractions.Integration.Implementations;
using Announcarr.Integrations.Radarr.Integration.Configurations;
using Announcarr.Integrations.Radarr.Integration.Contracts;
using Announcarr.Utils.Extensions.DateTime;
using Microsoft.Extensions.Logging;

namespace Announcarr.Integrations.Radarr.Integration.Services;

public class RadarrIntegrationService : BaseIntegrationService<RadarrIntegrationConfiguration>
{
    public RadarrIntegrationService(RadarrIntegrationConfiguration configuration) : this(null, configuration)
    {
    }

    public RadarrIntegrationService(ILogger<RadarrIntegrationService>? logger, RadarrIntegrationConfiguration configuration) : base(logger, configuration)
    {
    }

    public override bool IsEnabled => Configuration.IsEnabled;

    public override string Name => Configuration.Name ?? "Radarr";

    protected override async Task<ForecastContract> GetForecastLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using var radarrApiClient = new RadarrApiClient(Configuration.Url, Configuration.ApiKey!, Configuration.IgnoreCertificateValidation);

        return new ForecastContract
        {
            Items = await GetForecastItems(radarrApiClient, from, to, false, cancellationToken),
        };
    }

    protected override async Task<SummaryContract> GetSummaryLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using IRadarrApiClient radarrApiClient = new RadarrApiClient(Configuration.Url, Configuration.ApiKey!, Configuration.IgnoreCertificateValidation);

        return new SummaryContract
        {
            NewlyMonitoredItems = await GetNewlyMonitoredItemsAsync(radarrApiClient, from, to, cancellationToken),
            NewItems = await GetForecastItems(radarrApiClient, from, to, true, cancellationToken),
        };
    }

    private async Task<List<BaseItem>> GetForecastItems(IRadarrApiClient radarrApiClient, DateTimeOffset from, DateTimeOffset to, bool onlyMoviesWithFile, CancellationToken cancellationToken)
    {
        List<MovieResource> episodeResources = await radarrApiClient.GetCalendarAsync(from, to, cancellationToken: cancellationToken);

        return episodeResources.Where(movie => !onlyMoviesWithFile || (movie.HasFile ?? false)).Select(movie => ToRadarrItem(movie, from, to)).Cast<BaseItem>().ToList();
    }

    private async Task<List<NewlyMonitoredItem>> GetNewlyMonitoredItemsAsync(IRadarrApiClient radarrApiClient, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        List<MovieResource> movieResources = await radarrApiClient.GetMoviesAsync(cancellationToken: cancellationToken);

        return movieResources.Where(movie => movie.Added?.Between(from.DateTime, to) ?? false).Select(movie => ToNewlyMonitoredMovie(movie, from, to)).Cast<NewlyMonitoredItem>().ToList();
    }

    private RadarrItem ToRadarrItem(MovieResource movie, DateTimeOffset from, DateTimeOffset to)
    {
        (DateTimeOffset? relevantDate, ReleaseDateType relevantDateType) = GetRelevantDate(movie, from, to);

        return new RadarrItem
        {
            ItemSource = Name,
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
            ItemSource = Name,
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