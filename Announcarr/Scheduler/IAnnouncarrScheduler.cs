namespace Announcarr.Scheduler;

public interface IAnnouncarrScheduler
{
    Task StartSchedulerAsync(CancellationToken cancellationToken = default);
}