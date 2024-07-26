using Announcarr.Integrations.Abstractions.Interfaces;
using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Integrations.Abstractions.AbstractImplementations;

public abstract class BaseIntegrationService : IIntegrationService
{
    public abstract bool IsEnabled { get; }
    public abstract string Name { get; }
    public abstract bool IsGetCalendarEnabled { get; }

    public Task<CalendarResponse> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (!IsGetCalendarEnabled)
        {
            return Task.FromResult(new CalendarResponse());
        }

        return GetCalendarLogicAsync(from, to, cancellationToken);
    }

    protected abstract Task<CalendarResponse> GetCalendarLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    public abstract bool IsGetRecentlyAddedEnabled { get; }

    public Task<RecentlyAddedResponse> GetRecentlyAddedAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (!IsGetRecentlyAddedEnabled)
        {
            return Task.FromResult(new RecentlyAddedResponse());
        }

        return GetRecentlyAddedLogicAsync(from, to, cancellationToken);
    }

    protected abstract Task<RecentlyAddedResponse> GetRecentlyAddedLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}