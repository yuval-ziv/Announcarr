using Announcarr.Abstractions.Contracts.Contracts;

namespace Announcarr.Exporters.Abstractions.Exporter.Interfaces;

public interface IExporterService
{
    bool IsEnabled { get; }
    string Name { get; }
    bool IsTestExporterEnabled { get; }
    bool IsExportCalendarEnabled { get; }
    bool IsExportRecentlyAddedEnabled { get; }
    Task TestExporterAsync(CancellationToken cancellationToken = default);
    Task ExportCalendarAsync(CalendarContract calendarContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    Task ExportRecentlyAddedAsync(RecentlyAddedContract recentlyAddedContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
}