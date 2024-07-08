using Announcer.Clients.Radarr.Client;
using Announcer.Clients.Radarr.Responses;
using Announcer.Integrations.Abstractions.Interfaces;
using Announcer.Integrations.Abstractions.Responses;
using Announcer.Integrations.Radarr.Configurations;
using Announcer.Integrations.Radarr.Contracts;

namespace Announcer.Integrations.Radarr.Services;

public class RadarrIntegrationService : IIntegrationService
{
    private readonly RadarrIntegrationConfiguration _configuration;

    public RadarrIntegrationService(RadarrIntegrationConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetName() => "Radarr";

    public async Task<CalendarResponse> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using var radarrApiClient = new RadarrApiClient(_configuration.Url, _configuration.ApiKey!, _configuration.IgnoreCertificateValidation);
        List<MovieResource> episodeResources = await radarrApiClient.GetCalendarAsync(from, to, cancellationToken: cancellationToken);

        return new CalendarResponse { CalendarItems = episodeResources.Select(ToSonarrCalendarItem).Cast<BaseCalendarItem>().ToList() };
    }

    private RadarrCalendarItem ToSonarrCalendarItem(MovieResource movie)
    {
        return new RadarrCalendarItem
        {
            CalendarItemSource = GetName(),
            ThumbnailUrl = movie.Images?.FirstOrDefault(cover => cover.CoverType == MediaCoverTypes.Poster)?.RemoteUrl ?? string.Empty,
            MovieName = movie.Title,
        };
    }

    public Task GetNewAnnouncementAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}