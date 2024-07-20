using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Exporters.Abstractions.Exporter.Interfaces;

public interface IExporterService
{
    bool IsEnabled();
    string GetName();
    Task TestExporterAsync(CancellationToken cancellationToken = default);

    Task ExportCalendarAsync(CalendarResponse calendarResponse, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
}