using Announcarr.Abstractions.Contracts;
using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Microsoft.Extensions.Logging;

namespace Announcarr.Exporters.Abstractions.Exporter.AbstractImplementations;

public abstract class BaseExporterService<TConfiguration> : IExporterService where TConfiguration : BaseExporterConfiguration
{
    protected readonly ILogger<BaseExporterService<TConfiguration>>? Logger;
    protected readonly TConfiguration Configuration;

    protected BaseExporterService(TConfiguration configuration) : this(null, configuration)
    {
    }

    protected BaseExporterService(ILogger<BaseExporterService<TConfiguration>>? logger, TConfiguration configuration)
    {
        Logger = logger;
        Configuration = configuration;
    }

    public abstract bool IsEnabled { get; }
    public abstract string Name { get; }
    public abstract bool? ExportOnEmptyContract { get; set; }
    public abstract string? CustomMessageOnEmptyContract { get; set; }

    public async Task TestExporterAsync(CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug("Testing exporter {ExporterName}", Name);
        if (!Configuration.IsEnabledByAnnouncementType(AnnouncementType.Test))
        {
            Logger?.LogDebug("Exporter {ExporterName} is disabled for announcement type {AnnouncementType}", Name, AnnouncementType.Test);
            return;
        }

        await TestExporterLogicAsync(cancellationToken);
        Logger?.LogDebug("Finished testing exporter {ExporterName}", Name);
    }

    public Task ExportForecastAsync(IEnumerable<ForecastContract> contracts, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        ForecastContract mergedContract = contracts.Where(IsTagSupportedByExporter).Aggregate(ForecastContract.Merge);

        return ExportForecastAsync(mergedContract, startDate, endDate, cancellationToken);
    }

    public Task ExportSummaryAsync(IEnumerable<SummaryContract> contracts, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        SummaryContract mergedContract = contracts.Where(IsTagSupportedByExporter).Aggregate(SummaryContract.Merge);

        return ExportSummaryAsync(mergedContract, startDate, endDate, cancellationToken);
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

    protected Task ExportForecastAsync(ForecastContract contract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug("Start to export {AnnouncementType} with {ItemsCount} items between {Start} and {End} on exporter {ExporterName}", contract.AnnouncementType, contract.Items.Count,
            startDate, endDate, Name);
        if (!Configuration.IsEnabledByAnnouncementType(contract.AnnouncementType))
        {
            Logger?.LogDebug("Not exporting items on exporter {ExporterName} because announcement type {AnnouncementType} is not enabled", Name, contract.AnnouncementType);
            return Task.CompletedTask;
        }

        if (!IsTagSupportedByExporter(contract))
        {
            Logger?.LogDebug("Not exporting items on exporter {ExporterName} because tag is not supported", Name);
            return Task.CompletedTask;
        }

        if (!contract.IsEmpty)
        {
            Logger?.LogDebug("Exporting items on exporter {ExporterName}", Name);
            return ExportForecastLogicAsync(contract, startDate, endDate, cancellationToken);
        }

        if (ExportOnEmptyContract ?? false)
        {
            Logger?.LogDebug("Exporting empty contract on exporter {ExporterName}", Name);
            return ExportEmptyForecastLogicAsync(startDate, endDate, cancellationToken);
        }

        Logger?.LogDebug("Not exporting items on exporter {ExporterName} because contract is empty", Name);
        return Task.CompletedTask;
    }

    protected Task ExportSummaryAsync(SummaryContract contract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug("Start to export {AnnouncementType} with {NewItemsCount} items and {NewlyMonitoredItemsCount} items between {Start} and {End} on exporter {ExporterName}",
            contract.AnnouncementType, contract.NewItems.Count, contract.NewlyMonitoredItems.Count, startDate, endDate, Name);
        if (!Configuration.IsEnabledByAnnouncementType(AnnouncementType.Summary))
        {
            Logger?.LogDebug("Not exporting items on exporter {ExporterName} because announcement type {AnnouncementType} is not enabled", Name, contract.AnnouncementType);
            return Task.CompletedTask;
        }

        if (!IsTagSupportedByExporter(contract))
        {
            Logger?.LogDebug("Not exporting items on exporter {ExporterName} because tag is not supported", Name);
            return Task.CompletedTask;
        }

        if (!contract.IsEmpty)
        {
            Logger?.LogDebug("Exporting items on exporter {ExporterName}", Name);
            return ExportSummaryLogicAsync(contract, startDate, endDate, cancellationToken);
        }

        if (ExportOnEmptyContract ?? false)
        {
            Logger?.LogDebug("Exporting empty contract on exporter {ExporterName}", Name);
            return ExportEmptySummaryLogicAsync(startDate, endDate, cancellationToken);
        }

        Logger?.LogDebug("Not exporting items on exporter {ExporterName} because contract is empty", Name);
        return Task.CompletedTask;
    }

    protected abstract Task TestExporterLogicAsync(CancellationToken cancellationToken = default);

    protected abstract Task ExportForecastLogicAsync(ForecastContract forecastContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    protected abstract Task ExportEmptyForecastLogicAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);

    protected abstract Task ExportSummaryLogicAsync(SummaryContract summaryContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    protected abstract Task ExportEmptySummaryLogicAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
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