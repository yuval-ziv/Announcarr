using Announcarr.Abstractions.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class TestNotificationOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    public NotificationType NotificationType => NotificationType.TestNotification;

    public CustomAnnouncement Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        return new CustomAnnouncement
        {
            Title = contract.Subject,
            Message = contract.Message,
        };
    }
}