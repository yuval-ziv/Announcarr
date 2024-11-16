namespace Announcarr.Webhooks.Overseerr.Webhook.Contracts;

public class MediaRequest
{
    public required int RequestId { get; set; }
    public required string ReportedByUsername { get; set; }
    public required string ReportedByEmail { get; set; }
    public required string ReportedByAvatar { get; set; }
}