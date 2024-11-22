using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;

namespace Announcarr.Webhooks.Overseerr.Webhook.Contracts;

public class OverseerrWebhookContract
{
    public NotificationType NotificationType { get; set; }
    public string? Event { get; set; }
    public required string Subject { get; set; }
    public bool NotifySystem { get; set; }
    public Media? Media { get; set; }
    public string? Image { get; set; }
    public string? Message { get; set; }
    public List<ExtraData>? Extra { get; set; }
    public MediaRequest? Request { get; set; }
    public Issue? Issue { get; set; }
    public IssueComment? Comment { get; set; }
}

public class ExtraData
{
    public string? Name { get; set; }
    public string? Value { get; set; }
}