using Announcarr.Abstractions.Contracts;
using Announcarr.Clients.Sonarr.Client;
using Announcarr.Clients.Sonarr.Responses;
using Announcarr.Integrations.Abstractions.Integration.Implementations;
using Announcarr.Integrations.Sonarr.Integration.Configurations;
using Announcarr.Integrations.Sonarr.Integration.Contracts;
using Announcarr.Utils.Extensions.DateTime;

namespace Announcarr.Integrations.Sonarr.Integration.Services;

public class SonarrIntegrationService : BaseIntegrationService<SonarrIntegrationConfiguration>
{
    public SonarrIntegrationService(SonarrIntegrationConfiguration configuration) : base(configuration)
    {
    }

    public override bool IsEnabled => Configuration.IsEnabled;

    public override string Name => Configuration.Name ?? "Sonarr";

    protected override async Task<ForecastContract> GetForecastLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using var sonarrApiClient = new SonarrApiClient(Configuration.Url, Configuration.ApiKey!, Configuration.IgnoreCertificateValidation);
        List<EpisodeResource> episodeResources = await sonarrApiClient.GetCalendarAsync(from, to, includeSeries: true, cancellationToken: cancellationToken);

        return new ForecastContract { Items = episodeResources.GroupBy(resource => resource.Series?.Title).SelectMany(ToSonarrItem).Cast<BaseItem>().ToList() };
    }

    protected override async Task<SummaryContract> GetSummaryLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using var sonarrApiClient = new SonarrApiClient(Configuration.Url, Configuration.ApiKey!, Configuration.IgnoreCertificateValidation);

        List<SeriesResource> seriesResources = await sonarrApiClient.GetSeriesAsync(includeSeasonImages: true, cancellationToken: cancellationToken);

        List<EpisodeResource> episodeResources = await sonarrApiClient.GetCalendarAsync(from, to, includeSeries: true, cancellationToken: cancellationToken);

        return new SummaryContract
        {
            NewlyMonitoredItems = seriesResources.Where(series => series.Added?.Between(from.DateTime, to) ?? false).Select(ToNewlyMonitoredSeries).Cast<NewlyMonitoredItem>().ToList(),
            NewItems = episodeResources.Where(episode => episode.HasFile).GroupBy(resource => resource.Series?.Title).SelectMany(ToSonarrItem).Cast<BaseItem>().ToList(),
        };
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