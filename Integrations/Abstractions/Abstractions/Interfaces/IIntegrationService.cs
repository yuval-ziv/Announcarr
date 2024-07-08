using Announcer.Integrations.Abstractions.Responses;

namespace Announcer.Integrations.Abstractions.Interfaces;

public interface IIntegrationService
{
    string GetName();
    Task<CalendarResponse> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task GetNewAnnouncementAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}