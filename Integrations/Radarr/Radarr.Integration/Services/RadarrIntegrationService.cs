using Announcarr.Clients.Radarr.Client;
using Announcarr.Clients.Radarr.Responses;
using Announcarr.Integrations.Abstractions.Interfaces;
using Announcarr.Integrations.Abstractions.Responses;
using Announcarr.Integrations.Radarr.Integration.Configurations;
using Announcarr.Integrations.Radarr.Integration.Contracts;

namespace Announcarr.Integrations.Radarr.Integration.Services;

public class RadarrIntegrationService : IIntegrationService
{
    private readonly RadarrIntegrationConfiguration _configuration;

    public RadarrIntegrationService(RadarrIntegrationConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsEnabled() => _configuration.IsEnabled;

    public string GetName() => _configuration.Name ?? "Radarr";

    public async Task<CalendarResponse> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        using var radarrApiClient = new RadarrApiClient(_configuration.Url, _configuration.ApiKey!, _configuration.IgnoreCertificateValidation);
        List<MovieResource> movieResources = await radarrApiClient.GetCalendarAsync(from, to, cancellationToken: cancellationToken);

        return new CalendarResponse { CalendarItems = movieResources.Select(movie => ToRadarrCalendarItem(movie, from, to)).Cast<BaseCalendarItem>().ToList() };
    }

    private RadarrCalendarItem ToRadarrCalendarItem(MovieResource movie, DateTimeOffset from, DateTimeOffset to)
    {
        (DateTimeOffset? relevantDate, ReleaseDateType relevantDateType) = GetRelevantDate(movie, from, to);

        return new RadarrCalendarItem
        {
            CalendarItemSource = GetName(),
            ReleaseDate = relevantDate,
            ReleaseDateType = relevantDateType,
            ThumbnailUrl = movie.Images?.FirstOrDefault(cover => cover.CoverType == MediaCoverTypes.Poster)?.RemoteUrl ?? string.Empty,
            MovieName = movie.Title,
        };
    }

    private (DateTimeOffset? relevantDate, ReleaseDateType relevantDateType) GetRelevantDate(MovieResource movie, DateTimeOffset from, DateTimeOffset to)
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

    public Task GetNewAnnouncementAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}