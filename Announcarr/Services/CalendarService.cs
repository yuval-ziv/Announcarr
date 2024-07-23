using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Announcarr.Integrations.Abstractions.Interfaces;
using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Services;

public class CalendarService : ICalendarService
{
    private readonly ILogger<CalendarService> _logger;
    private readonly List<IExporterService> _exporterServices;
    private readonly List<IIntegrationService> _integrationServices;

    public CalendarService(ILogger<CalendarService> logger, IEnumerable<IIntegrationService> integrationServices, IEnumerable<IExporterService> exporterServices)
    {
        _logger = logger;
        _exporterServices = exporterServices.ToList();
        _integrationServices = integrationServices.ToList();
    }

    public async Task<CalendarResponse> GetAllCalendarItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default)
    {
        start ??= DateTimeOffset.Now;
        end ??= start.Value.AddDays(7);

        CalendarResponse[] calendarResponses = await Task.WhenAll(_integrationServices.Where(integration => integration.IsEnabled)
            .Select(async serviceIntegration => await GetCalendarResponseAsync(serviceIntegration, start.Value, end.Value, cancellationToken)));
        CalendarResponse calendarResponse = calendarResponses.Length != 0 ? calendarResponses.Aggregate(CalendarResponse.Merge) : new CalendarResponse();

        if (export ?? false)
        {
            await Task.WhenAll(_exporterServices.Where(exporter => exporter.IsEnabled()).Select(exporter => exporter.ExportCalendarAsync(calendarResponse, start.Value, end.Value, cancellationToken)));
        }

        return calendarResponse;
    }

    public async Task<RecentlyAddedResponse> GetAllRecentlyAddedItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default)
    {
        start ??= DateTimeOffset.Now.AddDays(-7);
        end ??= start.Value.AddDays(7);

        RecentlyAddedResponse[] recentlyAddedResponses = await Task.WhenAll(_integrationServices.Where(integration => integration.IsEnabled)
            .Select(async serviceIntegration => await GetRecentlyAddedItemsAsync(serviceIntegration, start.Value, end.Value, cancellationToken)));
        RecentlyAddedResponse recentlyAddedResponse = recentlyAddedResponses.Length != 0 ? recentlyAddedResponses.Aggregate(RecentlyAddedResponse.Merge) : new RecentlyAddedResponse();

        if (export ?? false)
        {
            await Task.WhenAll(_exporterServices.Where(exporter => exporter.IsEnabled())
                .Select(exporter => exporter.ExportRecentlyAddedAsync(recentlyAddedResponse, start.Value, end.Value, cancellationToken)));
        }

        return recentlyAddedResponse;
    }

    private async Task<CalendarResponse> GetCalendarResponseAsync(IIntegrationService integrationService, DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken = default)
    {
        try
        {
            return await integrationService.GetCalendarAsync(start, end, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to get calendar items from {IntegrationServiceName}", integrationService.GetName());
            return new CalendarResponse();
        }
    }

    private async Task<RecentlyAddedResponse> GetRecentlyAddedItemsAsync(IIntegrationService integrationService, DateTimeOffset start, DateTimeOffset end,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await integrationService.GetRecentlyAddedAsync(start, end, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to get recently added items from {IntegrationServiceName}", integrationService.GetName());
            return new RecentlyAddedResponse();
        }
    }
}