using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Integrations.Abstractions.Interfaces;

public interface IIntegrationService
{
    bool IsEnabled { get; }
    string Name { get; }
    bool IsGetCalendarEnabled { get; }
    bool IsGetRecentlyAddedEnabled { get; }
    Task<CalendarResponse> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task<RecentlyAddedResponse> GetRecentlyAddedAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}