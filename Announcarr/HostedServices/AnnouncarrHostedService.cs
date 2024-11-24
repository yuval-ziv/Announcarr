using Announcarr.Scheduler;

namespace Announcarr.HostedServices;

public class AnnouncarrHostedService : IHostedService, IDisposable
{
    private readonly ILogger<AnnouncarrHostedService> _logger;
    private readonly IAnnouncarrScheduler _scheduler;
    private readonly TaskFactory _taskFactory;
    private readonly CancellationTokenSource _cts;

    public AnnouncarrHostedService(ILogger<AnnouncarrHostedService> logger, IAnnouncarrScheduler scheduler)
    {
        _logger = logger;
        _scheduler = scheduler;

        _cts = new CancellationTokenSource();
        _taskFactory = new TaskFactory(_cts.Token, TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning, TaskScheduler.Current);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting Announcarr hosted service");

        _taskFactory.StartNew(async () => await _scheduler.StartSchedulerAsync(_cts.Token), _cts.Token);

        _logger.LogDebug("Started Announcarr hosted service");
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Stopping Announcarr hosted service");

        await _cts.CancelAsync();

        _logger.LogDebug("Stopped Announcarr hosted service");
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}