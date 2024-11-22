using Announcarr.Abstractions.Contracts;
using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Microsoft.Extensions.Logging;

namespace Announcarr.Exporters.Abstractions.Exporter.AbstractImplementations;

public abstract class BaseExporterService<TConfiguration> : IExporterService where TConfiguration : BaseExporterConfiguration
{
    private readonly ILogger<BaseExporterService<TConfiguration>>? _logger;
    protected readonly TConfiguration Configuration;

    protected BaseExporterService(TConfiguration configuration) : this(null, configuration)
    {
    }

    protected BaseExporterService(ILogger<BaseExporterService<TConfiguration>>? logger, TConfiguration configuration)
    {
        _logger = logger;
        Configuration = configuration;
    }

    public abstract bool IsEnabled { get; }
    public abstract string Name { get; }
    public abstract bool? ExportOnEmptyContract { get; set; }
    public abstract string? CustomMessageOnEmptyContract { get; set; }

    public async Task TestExporterAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Testing exporter {ExporterName}", Name);
        if (!Configuration.IsEnabledByAnnouncementType(AnnouncementType.Test))
        {
            _logger?.LogDebug("Exporter {ExporterName} is disabled for announcement type {AnnouncementType}", Name, AnnouncementType.Test);
            return;
        }

        await TestExporterLogicAsync(cancellationToken);
        _logger?.LogDebug("Finished testing exporter {ExporterName}", Name);
    }

    public Task ExportCalendarAsync(IEnumerable<CalendarContract> calendarContracts, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        CalendarContract mergedContract = calendarContracts.Where(IsTagSupportedByExporter).Aggregate(CalendarContract.Merge);

        return ExportCalendarAsync(mergedContract, startDate, endDate, cancellationToken);
    }

    public Task ExportRecentlyAddedAsync(IEnumerable<RecentlyAddedContract> recentlyAddedContracts, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        RecentlyAddedContract mergedContract = recentlyAddedContracts.Where(IsTagSupportedByExporter).Aggregate(RecentlyAddedContract.Merge);

        return ExportRecentlyAddedAsync(mergedContract, startDate, endDate, cancellationToken);
    }

    public Task ExportCustomAnnouncementAsync(CustomAnnouncement customAnnouncement, CancellationToken cancellationToken = default)
    {
        if (!Configuration.IsEnabledByAnnouncementType(AnnouncementType.Announcement))
        {
            return Task.CompletedTask;
        }

        if (!IsTagSupportedByExporter(customAnnouncement))
        {
            return Task.CompletedTask;
        }

        if (!customAnnouncement.IsEmpty)
        {
            return ExportAnnouncementLogicAsync(customAnnouncement, cancellationToken);
        }

        return Task.CompletedTask;
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
    protected abstract Task ExportAnnouncementLogicAsync(CustomAnnouncement message, CancellationToken cancellationToken = default);

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