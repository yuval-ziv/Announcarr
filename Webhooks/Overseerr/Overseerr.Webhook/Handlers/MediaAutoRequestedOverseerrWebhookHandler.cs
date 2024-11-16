using Announcarr.Abstractions.Contracts;
using Announcarr.Utils.Extensions.String;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class MediaAutoRequestedOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    private readonly ILogger<MediaAutoRequestedOverseerrWebhookHandler> _logger;

    public MediaAutoRequestedOverseerrWebhookHandler(ILogger<MediaAutoRequestedOverseerrWebhookHandler> logger)
    {
        _logger = logger;
    }

    public NotificationType NotificationType => NotificationType.MediaAutoRequested;

    public CustomAnnouncement? Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        if (contract.Request is null || contract.Media is null)
        {
            _logger.LogError("Malformed webhook contract. Null checks - Contract.Request={RequestIsNull},Contract.Media={MediaIsNull}", contract.Request is null, contract.Media is null);
            return null;
        }

        string username = contract.Request.ReportedByUsername;
        string email = contract.Request.ReportedByEmail;
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