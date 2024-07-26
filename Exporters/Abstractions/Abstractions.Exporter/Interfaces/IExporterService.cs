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
    Task ExportCalendarAsync(CalendarResponse calendarResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    Task ExportRecentlyAddedAsync(RecentlyAddedResponse recentlyAddedResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
}