using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;

namespace Announcarr.Webhooks.Overseerr.Extensions.Configurations;

public class OverseerrWebhookConfiguration
{
    public string? Name { get; set; }
    public bool IsEnabled { get; set; } = true;
    public Dictionary<NotificationType, NotificationTypeConfiguration> NotificationTypeToConfiguration { get; set; } = [];
}