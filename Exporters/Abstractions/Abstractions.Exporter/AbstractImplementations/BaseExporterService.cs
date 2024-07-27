using Announcarr.Abstractions.Contracts.Contracts;
using Announcarr.Exporters.Abstractions.Exporter.Interfaces;

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

    public abstract bool IsExportCalendarEnabled { get; }

    public Task ExportCalendarAsync(CalendarContract calendarContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        if (!IsTestExporterEnabled)
        {
            return Task.CompletedTask;
        }

        return ExportCalendarLogicAsync(calendarContract, startDate, endDate, cancellationToken);
    }

    public abstract bool IsExportRecentlyAddedEnabled { get; }

    public Task ExportRecentlyAddedAsync(RecentlyAddedContract recentlyAddedContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        if (!IsTestExporterEnabled)
        {
            return Task.CompletedTask;
        }

        return ExportRecentlyAddedLogicAsync(recentlyAddedContract, startDate, endDate, cancellationToken);
    }

    protected abstract Task TestExporterLogicAsync(CancellationToken cancellationToken = default);

    protected abstract Task ExportCalendarLogicAsync(CalendarContract calendarContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);

    protected abstract Task ExportRecentlyAddedLogicAsync(RecentlyAddedContract recentlyAddedContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
}