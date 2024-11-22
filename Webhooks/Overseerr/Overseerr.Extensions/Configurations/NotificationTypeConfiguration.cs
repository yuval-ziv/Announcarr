namespace Announcarr.Webhooks.Overseerr.Extensions.Configurations;

public class NotificationTypeConfiguration
{
    public bool IsEnabled { get; set; } = true;
    public HashSet<string> Tags { get; set; } = [];
}