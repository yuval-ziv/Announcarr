using Announcarr.Abstractions.Contracts;
using Announcarr.Clients.Sonarr.Client;
using Announcarr.Clients.Sonarr.Responses;
using Announcarr.Integrations.Abstractions.Integration.Implementations;
using Announcarr.Integrations.Sonarr.Integration.Configurations;
using Announcarr.Integrations.Sonarr.Integration.Contracts;
using Announcarr.Utils.Extensions.DateTime;
using Microsoft.Extensions.Logging;

namespace Announcarr.Integrations.Sonarr.Integration.Services;

public class SonarrIntegrationService : BaseIntegrationService<SonarrIntegrationConfiguration>
{
    public SonarrIntegrationService(SonarrIntegrationConfiguration configuration) : this(null, configuration)
    {
    }

    public SonarrIntegrationService(ILogger<SonarrIntegrationService>? logger, SonarrIntegrationConfiguration configuration) : base(logger, configuration)
    {
    }

    public override bool IsEnabled => Configuration.IsEnabled;

    public override string Name => Configuration.Name ?? "Sonarr";

    protected override async Task<ForecastContract> GetForecastLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using var sonarrApiClient = new SonarrApiClient(Configuration.Url, Configuration.ApiKey!, Configuration.IgnoreCertificateValidation);
        List<BaseItem> items = await GetForecastItemsAsync(sonarrApiClient, from, to, false, cancellationToken);

        return new ForecastContract
        {
            Items = items,
        };
    }

    protected override async Task<SummaryContract> GetSummaryLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using var sonarrApiClient = new SonarrApiClient(Configuration.Url, Configuration.ApiKey!, Configuration.IgnoreCertificateValidation);

        return new SummaryContract
        {
            NewItems = await GetForecastItemsAsync(sonarrApiClient, from, to, true, cancellationToken),
            NewlyMonitoredItems = await GetNewlyMonitoredItemsAsync(sonarrApiClient, from, to, cancellationToken),
        };
    }

    private async Task<List<BaseItem>> GetForecastItemsAsync(SonarrApiClient sonarrApiClient, DateTimeOffset from, DateTimeOffset to, bool onlyEpisodesWithFile,
        CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug("Fetching calendar for {IntegrationName} between {From} to {To}", Name, from, to);
        List<EpisodeResource> episodeResources = await sonarrApiClient.GetCalendarAsync(from, to, includeSeries: true, cancellationToken: cancellationToken);
        List<BaseItem> items = episodeResources.Where(episode => !onlyEpisodesWithFile || (episode.HasFile ?? false)).GroupBy(resource => resource.Series?.Title).SelectMany(ToSonarrItem)
            .Cast<BaseItem>().ToList();
        Logger?.LogDebug("Fetching calendar for {IntegrationName} between {From} to {To} finished. Found {Count} items", Name, from, to, items.Count);

        return items;
    }

    private async Task<List<NewlyMonitoredItem>> GetNewlyMonitoredItemsAsync(SonarrApiClient sonarrApiClient, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug("Fetching newly monitored items for {IntegrationName} between {From} to {To}", Name, from, to);
        List<SeriesResource> seriesResources = await sonarrApiClient.GetSeriesAsync(includeSeasonImages: true, cancellationToken: cancellationToken);
        List<NewlyMonitoredItem> newlyMonitoredItems =
            seriesResources.Where(series => series.Added?.Between(from.DateTime, to) ?? false).Select(ToNewlyMonitoredSeries).Cast<NewlyMonitoredItem>().ToList();
        Logger?.LogDebug("Fetching newly monitored items for {IntegrationName} between {From} to {To} finished. Found {Count} episodes resources", Name, from, to, newlyMonitoredItems.Count);

        return newlyMonitoredItems;
    }

    private IEnumerable<SonarrItem> ToSonarrItem(IGrouping<string?, EpisodeResource> seriesIdToEpisodes)
    {
        return seriesIdToEpisodes.GroupBy(resource => resource.AirDateUtc?.Date).Select(airDateToEpisodes => ToSonarrItem(seriesIdToEpisodes.Key, airDateToEpisodes));
    }

    private SonarrItem ToSonarrItem(string? seriesTitle, IGrouping<DateTime?, EpisodeResource> seriesIdToEpisodes)
    {
        return new SonarrItem
        {
            ItemSource = Name,
            ReleaseDate = seriesIdToEpisodes.FirstOrDefault(resource => resource.AirDateUtc is not null)?.AirDateUtc,
            ThumbnailUrl = GetThumbnailUrl(seriesIdToEpisodes.FirstOrDefault()?.Series),
            SeriesName = seriesTitle,
            Seasons = seriesIdToEpisodes.GroupBy(episode => episode.SeasonNumber).Select(ToSeason).ToList(),
        };
    }

    private static Season ToSeason(IGrouping<int, EpisodeResource> seasonNumberToEpisodes)
    {
        int seasonNumber = seasonNumberToEpisodes.Key;
        return new Season
        {
            SeasonNumber = seasonNumber,
            Episodes = seasonNumberToEpisodes.Select(ToEpisode).ToList(),
        };
    }

    private static Episode ToEpisode(EpisodeResource episode)
    {
        return new Episode
        {
            EpisodeNumber = episode.EpisodeNumber,
            EpisodeTitle = episode.Title,
        };
    }

    private NewlyMonitoredSeries ToNewlyMonitoredSeries(SeriesResource series)
    {
        return new NewlyMonitoredSeries
        {
            ItemSource = Name,
            StartedMonitoring = series.Added,
            ThumbnailUrl = GetThumbnailUrl(series),
            SeriesName = series.Title,
            SeasonToAvailableEpisodesCount = GetSeasonToAvailableEpisodesCount(series.Seasons),
        };
    }

    private static string? GetThumbnailUrl(SeriesResource? series)
    {
        return series?.Images?.FirstOrDefault(cover => cover.CoverType == MediaCoverTypes.Poster)?.RemoteUrl;
    }

    private List<SeasonEpisodeCount> GetSeasonToAvailableEpisodesCount(List<SeasonResource>? seasons)
    {
        return seasons?.Where(season => season.SeasonNumber != 0 || !Configuration.IgnoreSeasonZero).Select(GetAvailableEpisodesCount).ToList() ?? [];
    }

    private static SeasonEpisodeCount GetAvailableEpisodesCount(SeasonResource season)
    {
        return new SeasonEpisodeCount
        {
            SeasonNumber = season.SeasonNumber,
            AvailableEpisodesCount = season.Statistics.EpisodeCount,
            TotalSeasonEpisodesCount = season.Statistics.TotalEpisodeCount,
        };
    }
}