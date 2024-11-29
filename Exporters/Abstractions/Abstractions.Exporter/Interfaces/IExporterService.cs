using Announcarr.Abstractions.Contracts;

namespace Announcarr.Exporters.Abstractions.Exporter.Interfaces;

public interface IExporterService
{
    bool IsEnabled { get; }
    string Name { get; }
    bool? ExportOnEmptyContract { get; set; }
    string? CustomMessageOnEmptyContract { get; set; }
    Task TestExporterAsync(CancellationToken cancellationToken = default);
    Task ExportForecastAsync(IEnumerable<ForecastContract> contracts, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    Task ExportSummaryAsync(IEnumerable<SummaryContract> contracts, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    Task ExportCustomAnnouncementAsync(CustomAnnouncement customAnnouncement, CancellationToken cancellationToken = default);
}