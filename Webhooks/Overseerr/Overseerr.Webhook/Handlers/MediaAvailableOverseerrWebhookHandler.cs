using Announcarr.Abstractions.Contracts;
using Announcarr.Utils.Extensions.String;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;
using Announcarr.Webhooks.Overseerr.Webhook.Exceptions;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class MediaAvailableOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    public NotificationType NotificationType => NotificationType.MediaAvailable;

    public CustomAnnouncement? Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        if (contract.Media is null)
        {
            throw new WebhookMalformedContractException($"Malformed webhook contract. Null checks - Contract.Media={contract.Media is null}");
        }

        string mediaType = contract.Media.MediaType.ToString().ToLower();
        string subject = contract.Subject;

        // should look like - Deadpool (2016) (movie) is now available.
        var message = $"{subject} ({mediaType}) is now available.";

        // should look like movie/293660 or tv/95557
        string mediaItemLink = contract.Media.MediaType.ToString().ToLower() + "/" + (contract.Media.TmdbId.IsNullOrEmpty() ? contract.Media.TvdbId : contract.Media.TmdbId);

        return new CustomAnnouncement
        {
            Title = "Media item is now available",
            Message = message,
            Image = contract.Image,
            Link = mediaItemLink,
        };
    }
}