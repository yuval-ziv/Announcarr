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

    public async Task<ForecastContract> GetAllForecastItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default)
    {
        start ??= DateTimeOffset.Now;
        end ??= start.Value.AddDays(7);

        ForecastContract[] contracts = await Task.WhenAll(_integrationServices.Where(integration => integration.IsEnabled)
            .Select(async serviceIntegration => await GetForcastResponseAsync(serviceIntegration, start.Value, end.Value, cancellationToken)));

        if (export ?? false)
        {
            await Task.WhenAll(_exporterServices.Where(exporter => exporter.IsEnabled).Select(exporter => exporter.ExportForecastAsync(contracts, start.Value, end.Value, cancellationToken)));
        }

        return contracts.Length != 0 ? contracts.Aggregate(ForecastContract.Merge) : new ForecastContract();
    }

    public async Task<SummaryContract> GetAllSummaryItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default)
    {
        start ??= DateTimeOffset.Now.AddDays(-7);
        end ??= start.Value.AddDays(7);

        SummaryContract[] contracts = await Task.WhenAll(_integrationServices.Where(integration => integration.IsEnabled)
            .Select(async serviceIntegration => await GetSummaryItemsAsync(serviceIntegration, start.Value, end.Value, cancellationToken)));

        if (export ?? false)
        {
            await Task.WhenAll(_exporterServices.Where(exporter => exporter.IsEnabled)
                .Select(exporter => exporter.ExportSummaryAsync(contracts, start.Value, end.Value, cancellationToken)));
        }

        return contracts.Length != 0 ? contracts.Aggregate(SummaryContract.Merge) : new SummaryContract();
    }

    private async Task<ForecastContract> GetForcastResponseAsync(IIntegrationService integrationService, DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken = default)
    {
        try
        {
            return await integrationService.GetForecastAsync(start, end, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to get forecast items from {IntegrationServiceName}", integrationService.Name);
            return new ForecastContract();
        }
    }

    private async Task<SummaryContract> GetSummaryItemsAsync(IIntegrationService integrationService, DateTimeOffset start, DateTimeOffset end,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await integrationService.GetSummaryAsync(start, end, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to get summary items from {IntegrationServiceName}", integrationService.Name);
            return new SummaryContract();
        }
    }
}