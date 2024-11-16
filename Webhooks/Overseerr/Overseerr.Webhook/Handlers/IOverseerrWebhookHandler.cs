using Announcarr.Abstractions.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public interface IOverseerrWebhookHandler
{
    public NotificationType NotificationType { get; }
    public CustomAnnouncement? Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default);
}