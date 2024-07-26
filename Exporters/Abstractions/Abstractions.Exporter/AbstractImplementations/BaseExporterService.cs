using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Exporters.Abstractions.Exporter.AbstractImplementations;

public abstract class BaseExporterService : IExporterService
{
    public abstract bool IsEnabled { get; }
    public abstract string Name { get; }
    public abstract bool IsTestExporterEnabled { get; }

    public Task TestExporterAsync(CancellationToken cancellationToken = default)
    {
        if (!IsTestExporterEnabled)
        {
            return Task.CompletedTask;
        }

        return TestExporterLogicAsync(cancellationToken);
    }

    protected abstract Task TestExporterLogicAsync(CancellationToken cancellationToken = default);

    public abstract bool IsExportCalendarEnabled { get; }

    public Task ExportCalendarAsync(CalendarResponse calendarResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        if (!IsTestExporterEnabled)
        {
            return Task.CompletedTask;
        }

        return ExportCalendarLogicAsync(calendarResponse, startDate, endDate, cancellationToken);
    }

    protected abstract Task ExportCalendarLogicAsync(CalendarResponse calendarResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);

    public abstract bool IsExportRecentlyAddedEnabled { get; }

    public Task ExportRecentlyAddedAsync(RecentlyAddedResponse recentlyAddedResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        if (!IsTestExporterEnabled)
        {
            return Task.CompletedTask;
        }

        return ExportRecentlyAddedLogicAsync(recentlyAddedResponse, startDate, endDate, cancellationToken);
    }

    protected abstract Task ExportRecentlyAddedLogicAsync(RecentlyAddedResponse recentlyAddedResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
}