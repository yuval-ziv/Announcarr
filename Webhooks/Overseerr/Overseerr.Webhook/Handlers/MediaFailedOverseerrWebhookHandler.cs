using Announcarr.Abstractions.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class MediaFailedOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    private readonly ILogger<MediaFailedOverseerrWebhookHandler> _logger;

    public MediaFailedOverseerrWebhookHandler(ILogger<MediaFailedOverseerrWebhookHandler> logger)
    {
        _logger = logger;
    }

    public NotificationType NotificationType => NotificationType.MediaFailed;

    public CustomAnnouncement? Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Need to find out how a media failed event looks like before implementing it");
    }
}