using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;

namespace Announcarr.Webhooks.Overseerr.Extensions.Configurations;

public class OverseerrConfiguration
{
    public string? Name { get; set; } = null;
    public bool EnableWebhookListener { get; set; } = false;
    public bool AllowRemoteRequests { get; set; } = true;
    
    public string Method { get; set; } = HttpMethod.Post.Method;
    public string Path { get; set; } = "/overseerr/webhook";
    public string? AuthorizationHeader { get; set; } = string.Empty;
    
    public string OverseerrUrl { get; set; } = "http://localhost:5055";
    
    public bool IsEnabled { get; set; } = true;
    public Dictionary<NotificationType, NotificationTypeConfiguration> NotificationTypeToConfiguration { get; set; } = [];
}