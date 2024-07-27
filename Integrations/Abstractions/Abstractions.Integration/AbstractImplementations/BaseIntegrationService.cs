using Announcarr.Abstractions.Contracts.Contracts;
using Announcarr.Integrations.Abstractions.Interfaces;

namespace Announcarr.Integrations.Abstractions.AbstractImplementations;

public abstract class BaseIntegrationService : IIntegrationService
{
    public abstract bool IsEnabled { get; }
    public abstract string Name { get; }
    public abstract bool IsGetCalendarEnabled { get; }

    public Task<CalendarContract> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (!IsGetCalendarEnabled)
        {
            return Task.FromResult(new CalendarContract());
        }

        return GetCalendarLogicAsync(from, to, cancellationToken);
    }

    public abstract bool IsGetRecentlyAddedEnabled { get; }

    public Task<RecentlyAddedContract> GetRecentlyAddedAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (!IsGetRecentlyAddedEnabled)
        {
            return Task.FromResult(new RecentlyAddedContract());
        }

        return GetRecentlyAddedLogicAsync(from, to, cancellationToken);
    }

    protected abstract Task<CalendarContract> GetCalendarLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    protected abstract Task<RecentlyAddedContract> GetRecentlyAddedLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}