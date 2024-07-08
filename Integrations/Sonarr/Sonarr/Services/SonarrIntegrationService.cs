using Announcer.Clients.Sonarr.Client;
using Announcer.Clients.Sonarr.Responses;
using Announcer.Integrations.Abstractions.Interfaces;
using Announcer.Integrations.Abstractions.Responses;
using Announcer.Integrations.Sonarr.Configurations;
using Announcer.Integrations.Sonarr.Contracts;

namespace Announcer.Integrations.Sonarr.Services;

public class SonarrIntegrationService : IIntegrationService
{
    private readonly SonarrIntegrationConfiguration _configuration;

    public SonarrIntegrationService(SonarrIntegrationConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetName() => "Sonarr";

    public async Task<CalendarResponse> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using var sonarrApiClient = new SonarrApiClient(_configuration.Url, _configuration.ApiKey!, _configuration.IgnoreCertificateValidation);
        List<EpisodeResource> episodeResources = await sonarrApiClient.GetCalendarAsync(from, to, includeSeries: true, cancellationToken: cancellationToken);

        return new CalendarResponse { CalendarItems = episodeResources.GroupBy(resource => resource.Series?.Title).Select(ToSonarrCalendarItem).Cast<BaseCalendarItem>().ToList() };
    }

    private SonarrCalendarItem ToSonarrCalendarItem(IGrouping<string?, EpisodeResource> seriesIdToEpisodes)
    {
        string? seriesTitle = seriesIdToEpisodes.Key;

        return new SonarrCalendarItem
        {
            CalendarItemSource = GetName(),
            ThumbnailUrl = seriesIdToEpisodes.FirstOrDefault()?.Series?.Images?.FirstOrDefault(cover => cover.CoverType == MediaCoverTypes.Poster)?.RemoteUrl,
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

    public Task GetNewAnnouncementAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}