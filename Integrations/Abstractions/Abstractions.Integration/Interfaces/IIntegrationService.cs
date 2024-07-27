using Announcarr.Abstractions.Contracts.Contracts;

namespace Announcarr.Integrations.Abstractions.Interfaces;

public interface IIntegrationService
{
    bool IsEnabled { get; }
    string Name { get; }
    bool IsGetCalendarEnabled { get; }
    bool IsGetRecentlyAddedEnabled { get; }
    Task<CalendarContract> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task<RecentlyAddedContract> GetRecentlyAddedAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}