using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Integrations.Abstractions.Interfaces;

public interface IIntegrationService
{
    bool IsEnabled();
    string GetName();
    Task<CalendarResponse> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task GetNewAnnouncementAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}