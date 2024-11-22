using Announcarr.Abstractions.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;
using Announcarr.Webhooks.Overseerr.Webhook.Exceptions;
using Microsoft.Extensions.Logging;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class IssueCreatedOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    public NotificationType NotificationType => NotificationType.IssueCreated;

    public CustomAnnouncement Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        if (contract.Issue is null || contract.Media is null)
        {
            throw new WebhookMalformedContractException($"Malformed webhook contract. Null checks - Contract.Issue={contract.Issue is null},Contract.Media={contract.Media is null}");
        }

        string username = contract.Issue.ReportedByUsername;
        string email = contract.Issue.ReportedByEmail;
        string issueType = contract.Issue.IssueType.ToString().ToLower();
        string mediaType = contract.Media.MediaType.ToString().ToLower();
        string subject = contract.Subject;

        // should look like - User a_user (user@example.com) opened an issue of type audio for movie media item titled Deadpool (2016).
        var message = $"User {username} ({email}) opened an issue of type {issueType} for {mediaType} media item titled {subject}.";

        return new CustomAnnouncement
        {
            Title = "New issue reported",
            Message = message,
            Image = contract.Image,
            Link = $"issues/{contract.Issue.IssueId}",
        };
    }
}