using Announcarr.Abstractions.Contracts;

namespace Announcarr.Exporters.Abstractions.Exporter.Interfaces;

public interface IExporterService
{
    bool IsEnabled { get; }
    string Name { get; }
    bool? ExportOnEmptyContract { get; set; }
    string? CustomMessageOnEmptyContract { get; set; }
    Task TestExporterAsync(CancellationToken cancellationToken = default);
    Task ExportCalendarAsync(IEnumerable<CalendarContract> calendarContracts, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    Task ExportRecentlyAddedAsync(IEnumerable<RecentlyAddedContract> recentlyAddedContracts, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
}