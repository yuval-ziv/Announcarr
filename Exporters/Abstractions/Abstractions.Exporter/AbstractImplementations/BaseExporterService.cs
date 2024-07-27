using Announcarr.Abstractions.Contracts;
using Announcarr.Exporters.Abstractions.Exporter.Interfaces;

namespace Announcarr.Exporters.Abstractions.Exporter.AbstractImplementations;

public abstract class BaseExporterService<TConfiguration> : IExporterService where TConfiguration : BaseExporterConfiguration
{
    protected readonly TConfiguration Configuration;

    protected BaseExporterService(TConfiguration configuration)
    {
        Configuration = configuration;
    }

    public abstract bool IsEnabled { get; }
    public abstract string Name { get; }
    public abstract bool? ExportOnEmptyContract { get; set; }
    public abstract string? CustomMessageOnEmptyContract { get; set; }

    public Task TestExporterAsync(CancellationToken cancellationToken = default)
    {
        if (!Configuration.IsEnabledByAnnouncementType(AnnouncementType.Test))
        {
            return Task.CompletedTask;
        }

        return TestExporterLogicAsync(cancellationToken);
    }

    public Task ExportCalendarAsync(IEnumerable<CalendarContract> calendarContracts, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        CalendarContract mergedContract = calendarContracts.Where(IsTagSupportedByExporter).Aggregate(CalendarContract.Merge);

        return ExportCalendarAsync(mergedContract, startDate, endDate, cancellationToken);
    }

    protected Task ExportCalendarAsync(CalendarContract calendarContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        if (!Configuration.IsEnabledByAnnouncementType(calendarContract.AnnouncementType))
        {
            return Task.CompletedTask;
        }

        if (!IsTagSupportedByExporter(calendarContract))
        {
            return Task.CompletedTask;
        }

        if (!calendarContract.IsEmpty)
        {
            return ExportCalendarLogicAsync(calendarContract, startDate, endDate, cancellationToken);
        }

        if (ExportOnEmptyContract ?? false)
        {
            return ExportEmptyCalendarLogicAsync(startDate, endDate, cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task ExportRecentlyAddedAsync(IEnumerable<RecentlyAddedContract> recentlyAddedContracts, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        RecentlyAddedContract mergedContract = recentlyAddedContracts.Where(IsTagSupportedByExporter).Aggregate(RecentlyAddedContract.Merge);

        return ExportRecentlyAddedAsync(mergedContract, startDate, endDate, cancellationToken);
    }

    protected Task ExportRecentlyAddedAsync(RecentlyAddedContract recentlyAddedContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        if (!Configuration.IsEnabledByAnnouncementType(AnnouncementType.RecentlyAdded))
        {
            return Task.CompletedTask;
        }

        if (!IsTagSupportedByExporter(recentlyAddedContract))
        {
            return Task.CompletedTask;
        }

        if (!recentlyAddedContract.IsEmpty)
        {
            return ExportRecentlyAddedLogicAsync(recentlyAddedContract, startDate, endDate, cancellationToken);
        }

        if (ExportOnEmptyContract ?? false)
        {
            return ExportEmptyRecentlyAddedLogicAsync(startDate, endDate, cancellationToken);
        }

        return Task.CompletedTask;
    }

    protected abstract Task TestExporterLogicAsync(CancellationToken cancellationToken = default);

    protected abstract Task ExportCalendarLogicAsync(CalendarContract calendarContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    protected abstract Task ExportEmptyCalendarLogicAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);

    protected abstract Task ExportRecentlyAddedLogicAsync(RecentlyAddedContract recentlyAddedContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    protected abstract Task ExportEmptyRecentlyAddedLogicAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);

    protected virtual bool IsTagSupportedByExporter<TAnnouncement>(TAnnouncement announcement) where TAnnouncement : BaseAnnouncement
    {
        if (announcement.Tags.Count == 0)
        {
            return true;
        }

        HashSet<string> announcementTypeSupportedTags = Configuration.GetTagsByAnnouncementType(announcement.AnnouncementType);

        return announcementTypeSupportedTags.Overlaps(announcement.Tags);
    }
}