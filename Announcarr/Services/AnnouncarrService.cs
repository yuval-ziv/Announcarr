using Announcarr.Abstractions.Contracts;
using Announcarr.Configurations;
using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Announcarr.Integrations.Abstractions.Integration.Abstractions;
using Microsoft.Extensions.Options;

namespace Announcarr.Services;

public class AnnouncarrService : IAnnouncarrService
{
    private readonly List<IExporterService> _exporterServices;
    private readonly List<IIntegrationService> _integrationServices;
    private readonly ILogger<AnnouncarrService> _logger;

    public AnnouncarrService(ILogger<AnnouncarrService> logger, IOptionsMonitor<AnnouncarrConfiguration> options, IEnumerable<IIntegrationService> integrationServices,
        IEnumerable<IExporterService> exporterServices)
    {
        _logger = logger;
        AnnouncarrConfiguration configuration = options.CurrentValue;
        _exporterServices = exporterServices.ToList();
        _integrationServices = integrationServices.ToList();

        _exporterServices.ForEach(exporter =>
        {
            exporter.ExportOnEmptyContract = configuration.EmptyContractFallback.ExportOnEmptyContract;
            exporter.CustomMessageOnEmptyContract = configuration.EmptyContractFallback.CustomMessageOnEmptyContract;
        });
    }

    public async Task<CalendarContract> GetAllCalendarItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default)
    {
        start ??= DateTimeOffset.Now;
        end ??= start.Value.AddDays(7);

        CalendarContract[] calendarResponses = await Task.WhenAll(_integrationServices.Where(integration => integration.IsEnabled)
            .Select(async serviceIntegration => await GetCalendarResponseAsync(serviceIntegration, start.Value, end.Value, cancellationToken)));

        if (export ?? false)
        {
            await Task.WhenAll(_exporterServices.Where(exporter => exporter.IsEnabled).Select(exporter => exporter.ExportCalendarAsync(calendarResponses, start.Value, end.Value, cancellationToken)));
        }

        return calendarResponses.Length != 0 ? calendarResponses.Aggregate(CalendarContract.Merge) : new CalendarContract();
    }

    public async Task<RecentlyAddedContract> GetAllRecentlyAddedItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default)
    {
        start ??= DateTimeOffset.Now.AddDays(-7);
        end ??= start.Value.AddDays(7);

        RecentlyAddedContract[] recentlyAddedContracts = await Task.WhenAll(_integrationServices.Where(integration => integration.IsEnabled)
            .Select(async serviceIntegration => await GetRecentlyAddedItemsAsync(serviceIntegration, start.Value, end.Value, cancellationToken)));

        if (export ?? false)
        {
            await Task.WhenAll(_exporterServices.Where(exporter => exporter.IsEnabled)
                .Select(exporter => exporter.ExportRecentlyAddedAsync(recentlyAddedContracts, start.Value, end.Value, cancellationToken)));
        }

        return recentlyAddedContracts.Length != 0 ? recentlyAddedContracts.Aggregate(RecentlyAddedContract.Merge) : new RecentlyAddedContract();
    }

    private async Task<CalendarContract> GetCalendarResponseAsync(IIntegrationService integrationService, DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken = default)
    {
        try
        {
            return await integrationService.GetCalendarAsync(start, end, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to get calendar items from {IntegrationServiceName}", integrationService.Name);
            return new CalendarContract();
        }
    }

    private async Task<RecentlyAddedContract> GetRecentlyAddedItemsAsync(IIntegrationService integrationService, DateTimeOffset start, DateTimeOffset end,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await integrationService.GetRecentlyAddedAsync(start, end, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to get recently added items from {IntegrationServiceName}", integrationService.Name);
            return new RecentlyAddedContract();
        }
    }
}