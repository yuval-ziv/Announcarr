using Announcarr.Configurations;
using Announcarr.Services;
using Announcarr.Utils.Extensions;
using Microsoft.Extensions.Options;

namespace Announcarr.Scheduler;

public class AnnouncarrScheduler : IAnnouncarrScheduler
{
    private readonly ILogger<AnnouncarrScheduler> _logger;
    private readonly AnnouncarrConfiguration _configuration;
    private readonly IAnnouncarrService _announcarrService;

    public AnnouncarrScheduler(ILogger<AnnouncarrScheduler> logger, IOptionsMonitor<AnnouncarrConfiguration> options, IAnnouncarrService announcarrService)
    {
        _logger = logger;
        _configuration = options.CurrentValue;
        _announcarrService = announcarrService;
    }

    public async Task StartSchedulerAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started scheduler service. Announcarr range is {AnnouncarrRange}", _configuration.Interval.AnnouncarrRange);
        while (!cancellationToken.IsCancellationRequested)
        {
            await WaitToNextExecutionAsync(cancellationToken);

            await AnnounceSummary(cancellationToken);

            await AnnounceForecast(cancellationToken);
        }
    }

    private async Task WaitToNextExecutionAsync(CancellationToken cancellationToken)
    {
        DateTimeOffset nextExecution = _configuration.Interval.GetNextExecution();
        TimeSpan delay = nextExecution - DateTimeOffset.Now;
        _logger.LogInformation("Next execution in {NextExecution}. Waiting for {WaitTime}", nextExecution, delay);
        await Task.Delay(delay, cancellationToken);
    }

    private async Task AnnounceSummary(CancellationToken cancellationToken)
    {
        (DateTimeOffset start, DateTimeOffset end) = _configuration.Interval.GetLastRange();
        _logger.LogDebug("Announcing summary between {StartDate} and {EndDate}", start, end);
        await _announcarrService.GetAllRecentlyAddedItemsAsync(start, end, true, cancellationToken);
    }

    private async Task AnnounceForecast(CancellationToken cancellationToken)
    {
        (DateTimeOffset start, DateTimeOffset end) = _configuration.Interval.GetNextRange();
        _logger.LogDebug("Announcing forecast between {StartDate} and {EndDate}", start, end);
        await _announcarrService.GetAllCalendarItemsAsync(start, end, true, cancellationToken);
    }
}