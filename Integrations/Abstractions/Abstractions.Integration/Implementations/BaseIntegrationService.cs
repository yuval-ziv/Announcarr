using Announcarr.Abstractions.Contracts;
using Announcarr.Integrations.Abstractions.Integration.Abstractions;
using Microsoft.Extensions.Logging;

namespace Announcarr.Integrations.Abstractions.Integration.Implementations;

public abstract class BaseIntegrationService<TConfiguration> : IIntegrationService where TConfiguration : BaseIntegrationConfiguration
{
    protected readonly ILogger<BaseIntegrationService<TConfiguration>>? Logger;
    protected readonly TConfiguration Configuration;

    protected BaseIntegrationService(TConfiguration configuration) : this(null, configuration)
    {
    }

    protected BaseIntegrationService(ILogger<BaseIntegrationService<TConfiguration>>? logger, TConfiguration configuration)
    {
        Logger = logger;
        Configuration = configuration;
    }

    public abstract bool IsEnabled { get; }
    public abstract string Name { get; }

    public virtual async Task<ForecastContract> GetForecastAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug("Getting {AnnouncementType} from {IntegrationName} between {From} to {To}", AnnouncementType.Forecast, Name, from, to);
        if (!Configuration.IsEnabledByAnnouncementType(AnnouncementType.Forecast))
        {
            Logger?.LogDebug("Integration {IntegrationName} is disabled for {AnnouncementType}", Name, AnnouncementType.Forecast);
            return new ForecastContract();
        }

        ForecastContract contract = await GetForecastLogicAsync(from, to, cancellationToken);
        Logger?.LogDebug("Finished getting {AnnouncementType} from {IntegrationName} between {From} and {To}. Found {AmountOfItems} items in announcement {AnnouncementId}", AnnouncementType.Forecast,
            Name, from, to, contract.Items.Count, contract.Id);

        return AddTags(contract);
    }

    public virtual async Task<SummaryContract> GetSummaryAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug("Getting {AnnouncementType} from {IntegrationName} between {From} to {To}", AnnouncementType.Summary, Name, from, to);
        if (!Configuration.IsEnabledByAnnouncementType(AnnouncementType.Summary))
        {
            Logger?.LogDebug("Integration {IntegrationName} is disabled for {AnnouncementType}", Name, AnnouncementType.Summary);
            return new SummaryContract();
        }

        SummaryContract contract = await GetSummaryLogicAsync(from, to, cancellationToken);
        Logger?.LogDebug("Finished getting {AnnouncementType} from {IntegrationName} between {From} and {To}. " +
                         "Found {AmountOfNewItems} new items and {AmountOfNewlyAddedItems} newly monitored items in announcement {AnnouncementId}",
            AnnouncementType.Summary, Name, from, to, contract.NewItems.Count, contract.NewlyMonitoredItems.Count, contract.Id);

        return AddTags(contract);
    }

    protected abstract Task<ForecastContract> GetForecastLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    protected abstract Task<SummaryContract> GetSummaryLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    protected virtual TAnnouncement AddTags<TAnnouncement>(TAnnouncement announcement) where TAnnouncement : BaseAnnouncement
    {
        Logger?.LogDebug("Adding tags to announcement  {AnnouncementId}", announcement.Id);
        announcement.Tags = Configuration.GetTagsByAnnouncementType(announcement.AnnouncementType);
        Logger?.LogDebug("Finished adding tags to announcement {AnnouncementId} with {AmountOfTags} tags", announcement.Id, announcement.Tags.Count);
        return announcement;
    }
}