using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Exporters.Abstractions.Exporter.Interfaces;

public interface IExporterService
{
    bool IsEnabled();
    string GetName();
    bool IsTestExporterEnabled { get; }
    Task TestExporterAsync(CancellationToken cancellationToken = default);
    bool IsExportCalendarEnabled { get; }
    Task ExportCalendarAsync(CalendarResponse calendarResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    bool IsExportRecentlyAddedEnabled { get; }
    Task ExportRecentlyAddedAsync(RecentlyAddedResponse recentlyAddedResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
}