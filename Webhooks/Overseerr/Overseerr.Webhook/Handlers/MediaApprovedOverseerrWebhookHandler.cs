using Announcarr.Abstractions.Contracts;
using Announcarr.Utils.Extensions.String;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class MediaApprovedOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    private readonly ILogger<MediaApprovedOverseerrWebhookHandler> _logger;

    public MediaApprovedOverseerrWebhookHandler(ILogger<MediaApprovedOverseerrWebhookHandler> logger)
    {
        _logger = logger;
    }

    public NotificationType NotificationType => NotificationType.MediaApproved;

    public CustomAnnouncement? Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        if (contract.Media is null)
        {
            _logger.LogError("Malformed webhook contract. Null checks - Contract.Media={MediaIsNull}", contract.Media is null);
            return null;
        }

        string mediaType = contract.Media.MediaType.ToString().ToLower();
        string subject = contract.Subject;

        // should look like - Deadpool (2016) (movie) has been approved.
        var message = $"{subject} ({mediaType}) has been approved.";

        // should look like movie/293660 or tv/95557
        string mediaItemLink = contract.Media.MediaType.ToString().ToLower() + "/" + (contract.Media.TmdbId.IsNullOrEmpty() ? contract.Media.TvdbId : contract.Media.TmdbId);

        return new CustomAnnouncement
        {
            Title = "Media request approved",
            Message = message,
            Image = contract.Image,
            Link = mediaItemLink,
        };
    }
}