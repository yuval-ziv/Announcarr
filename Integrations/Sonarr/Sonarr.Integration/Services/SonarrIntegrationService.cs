using Announcarr.Clients.Sonarr.Client;
using Announcarr.Clients.Sonarr.Responses;
using Announcarr.Integrations.Abstractions.Interfaces;
using Announcarr.Integrations.Abstractions.Responses;
using Announcarr.Integrations.Sonarr.Integration.Configurations;
using Announcarr.Integrations.Sonarr.Integration.Contracts;
using Announcarr.Utils.Extensions.DateTime;

namespace Announcarr.Integrations.Sonarr.Integration.Services;

public class SonarrIntegrationService : IIntegrationService
{
    private readonly SonarrIntegrationConfiguration _configuration;

    public SonarrIntegrationService(SonarrIntegrationConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsEnabled => _configuration.IsEnabled;

    public string GetName() => _configuration.Name ?? "Sonarr";
    public bool IsGetCalendarEnabled => _configuration.IsGetCalendarEnabled;

    public async Task<CalendarResponse> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (!IsGetCalendarEnabled)
        {
            return new CalendarResponse();
        }

        using var sonarrApiClient = new SonarrApiClient(_configuration.Url, _configuration.ApiKey!, _configuration.IgnoreCertificateValidation);
        List<EpisodeResource> episodeResources = await sonarrApiClient.GetCalendarAsync(from, to, includeSeries: true, cancellationToken: cancellationToken);

        return new CalendarResponse { CalendarItems = episodeResources.GroupBy(resource => resource.Series?.Title).SelectMany(ToSonarrCalendarItem).Cast<BaseCalendarItem>().ToList() };
    }

    public bool IsGetRecentlyAddedEnabled => _configuration.IsGetRecentlyAddedEnabled;

    public async Task<RecentlyAddedResponse> GetRecentlyAddedAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (!IsGetRecentlyAddedEnabled)
        {
            return new RecentlyAddedResponse();
        }

        using var sonarrApiClient = new SonarrApiClient(_configuration.Url, _configuration.ApiKey!, _configuration.IgnoreCertificateValidation);

        List<SeriesResource> seriesResources = await sonarrApiClient.GetSeriesAsync(includeSeasonImages: true, cancellationToken: cancellationToken);

        List<EpisodeResource> episodeResources = await sonarrApiClient.GetCalendarAsync(from, to, includeSeries: true, cancellationToken: cancellationToken);

        return new RecentlyAddedResponse
        {
            NewlyMonitoredItems = seriesResources.Where(series => series.Added?.Between(from.DateTime, to) ?? false).Select(ToNewlyMonitoredSeries).Cast<NewlyMonitoredItem>().ToList(),
            NewItems = episodeResources.Where(episode => episode.HasFile).GroupBy(resource => resource.Series?.Title).SelectMany(ToSonarrCalendarItem).Cast<BaseCalendarItem>().ToList()
        };
    }

    private IEnumerable<SonarrCalendarItem> ToSonarrCalendarItem(IGrouping<string?, EpisodeResource> seriesIdToEpisodes)
    {
        return seriesIdToEpisodes.GroupBy(resource => resource.AirDateUtc?.Date).Select(airDateToEpisodes => ToSonarrCalendarItem(seriesIdToEpisodes.Key, airDateToEpisodes));
    }

    private SonarrCalendarItem ToSonarrCalendarItem(string? seriesTitle, IGrouping<DateTime?, EpisodeResource> seriesIdToEpisodes)
    {
        return new SonarrCalendarItem
        {
            CalendarItemSource = GetName(),
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

    private NewlyMonitoredSeries ToNewlyMonitoredSeries(SeriesResource seriesResource)
    {
        return new NewlyMonitoredSeries
        {
            CalendarItemSource = GetName(),
            StartedMonitoring = seriesResource.Added,
            ThumbnailUrl = GetThumbnailUrl(seriesResource),
            SeriesName = seriesResource.Title,
            SeasonToAvailableEpisodesCount = GetSeasonToAvailableEpisodesCount(seriesResource.Seasons),
        };
    }

    private static string? GetThumbnailUrl(SeriesResource? seriesResource)
    {
        return seriesResource?.Images?.FirstOrDefault(cover => cover.CoverType == MediaCoverTypes.Poster)?.RemoteUrl;
    }

    private List<SeasonEpisodeCount> GetSeasonToAvailableEpisodesCount(List<SeasonResource>? seasons)
    {
        return seasons?.Where(season => season.SeasonNumber != 0 || !_configuration.IgnoreSeasonZero).Select(GetAvailableEpisodesCount).ToList() ?? [];
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

    public Task GetNewAnnouncementAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}