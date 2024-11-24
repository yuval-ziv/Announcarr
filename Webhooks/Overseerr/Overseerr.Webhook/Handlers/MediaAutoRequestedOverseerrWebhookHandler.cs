using Announcarr.Abstractions.Contracts;
using Announcarr.Utils.Extensions.String;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;
using Announcarr.Webhooks.Overseerr.Webhook.Exceptions;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class MediaAutoRequestedOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    public NotificationType NotificationType => NotificationType.MediaAutoRequested;

    public CustomAnnouncement? Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        if (contract.Request is null || contract.Media is null)
        {
            throw new WebhookMalformedContractException($"Malformed webhook contract. Null checks - Contract.Request={contract.Request is null},Contract.Media={contract.Media is null}");
        }

        string username = contract.Request.RequestedByUsername;
        string email = contract.Request.RequestedByEmail;
        string mediaType = contract.Media.MediaType.ToString().ToLower();
        string subject = contract.Subject;

        // should look like - Deadpool (2016) (movie) has been auto requested by a_user (email@exmaple.com).
        var message = $"{subject} ({mediaType}) has been auto requested by {username} ({email}).";

        // should look like movie/293660 or tv/95557
        string mediaItemLink = contract.Media.MediaType.ToString().ToLower() + "/" + (contract.Media.TmdbId.IsNullOrEmpty() ? contract.Media.TvdbId : contract.Media.TmdbId);

        return new CustomAnnouncement
        {
            Title = "Media was auto requested (probably by plex watchlist)",
            Message = message,
            Image = contract.Image,
            Link = mediaItemLink,
        };
    }
}