namespace Announcarr.Webhooks.Overseerr.Webhook.Contracts;

public class MediaRequest
{
    public required int RequestId { get; set; }
    public required string RequestedByUsername { get; set; }
    public required string RequestedByEmail { get; set; }
    public required string RequestedByAvatar { get; set; }
}