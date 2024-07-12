using Announcarr.Integrations.Abstractions.Interfaces;
using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Services;

public class CalendarService : ICalendarService
{
    private readonly ILogger<CalendarService> _logger;
    private readonly List<IIntegrationService> _integrationServices;

    public CalendarService(ILogger<CalendarService> logger, IEnumerable<IIntegrationService> integrationServices)
    {
        _logger = logger;
        _integrationServices = integrationServices.ToList();
    }

    public async Task<CalendarResponse> GetAllCalendarItemsAsync(DateTimeOffset? start, DateTimeOffset? end, CancellationToken cancellationToken = default)
    {
        CalendarResponse[] calendarResponses = await Task.WhenAll(_integrationServices.Where(integration => integration.IsEnabled())
            .Select(async serviceIntegration => await GetCalendarResponseAsync(serviceIntegration, start, end, cancellationToken)));

        return calendarResponses.Aggregate(CalendarResponse.Merge);
    }

    private async Task<CalendarResponse> GetCalendarResponseAsync(IIntegrationService integrationService, DateTimeOffset? start, DateTimeOffset? end, CancellationToken cancellationToken = default)
    {
        try
        {
            return await integrationService.GetCalendarAsync(start ?? DateTimeOffset.Now, end ?? start?.AddDays(7) ?? DateTimeOffset.Now.AddDays(7), cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to get calendar items from {IntegrationServiceName}", integrationService.GetName());
            return new CalendarResponse();
        }
    }
}