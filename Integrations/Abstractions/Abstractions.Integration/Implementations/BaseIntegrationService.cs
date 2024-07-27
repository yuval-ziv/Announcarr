using Announcarr.Abstractions.Contracts;
using Announcarr.Integrations.Abstractions.Integration.Abstractions;

namespace Announcarr.Integrations.Abstractions.Integration.Implementations;

public abstract class BaseIntegrationService<TConfiguration> : IIntegrationService where TConfiguration : BaseIntegrationConfiguration
{
    protected readonly TConfiguration Configuration;

    protected BaseIntegrationService(TConfiguration configuration)
    {
        Configuration = configuration;
    }

    public abstract bool IsEnabled { get; }
    public abstract string Name { get; }

    public virtual async Task<CalendarContract> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (!Configuration.IsEnabledByAnnouncementType(AnnouncementType.Calendar))
        {
            return new CalendarContract();
        }

        return AddTags(await GetCalendarLogicAsync(from, to, cancellationToken));
    }

    public virtual async Task<RecentlyAddedContract> GetRecentlyAddedAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        if (!Configuration.IsEnabledByAnnouncementType(AnnouncementType.RecentlyAdded))
        {
            return new RecentlyAddedContract();
        }

        return AddTags(await GetRecentlyAddedLogicAsync(from, to, cancellationToken));
    }

    protected abstract Task<CalendarContract> GetCalendarLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    protected abstract Task<RecentlyAddedContract> GetRecentlyAddedLogicAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    protected virtual TAnnouncement AddTags<TAnnouncement>(TAnnouncement announcement) where TAnnouncement : BaseAnnouncement
    {
        announcement.Tags = Configuration.GetTagsByAnnouncementType(announcement.AnnouncementType);
        return announcement;
    }
}