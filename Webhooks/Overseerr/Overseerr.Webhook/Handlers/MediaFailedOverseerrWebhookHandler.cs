using Announcarr.Abstractions.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class MediaFailedOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    public NotificationType NotificationType => NotificationType.MediaFailed;

    public CustomAnnouncement Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Need to find out how a media failed event looks like before implementing it");
    }
}