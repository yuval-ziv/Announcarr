using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;

namespace Announcarr.Webhooks.Overseerr.Webhook.Contracts;

public class SeasonRequest
{
    public required int Id { get; set; }
    public required int SeasonNumber { get; set; }
    public required MediaRequestStatus Status { get; set; }
    public required MediaRequest Request { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}