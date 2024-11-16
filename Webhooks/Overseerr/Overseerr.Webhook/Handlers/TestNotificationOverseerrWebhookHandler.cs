using Announcarr.Abstractions.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class TestNotificationOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    private readonly ILogger<TestNotificationOverseerrWebhookHandler> _logger;

    public TestNotificationOverseerrWebhookHandler(ILogger<TestNotificationOverseerrWebhookHandler> logger)
    {
        _logger = logger;
    }

    public NotificationType NotificationType => NotificationType.TestNotification;

    public CustomAnnouncement? Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        return new CustomAnnouncement
        {
            Title = contract.Subject,
            Message = contract.Message,
        };
    }
}