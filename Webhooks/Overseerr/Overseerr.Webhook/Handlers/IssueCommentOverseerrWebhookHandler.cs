using Announcarr.Abstractions.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class IssueCommentOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    private readonly ILogger<IssueCommentOverseerrWebhookHandler> _logger;

    public IssueCommentOverseerrWebhookHandler(ILogger<IssueCommentOverseerrWebhookHandler> logger)
    {
        _logger = logger;
    }

    public NotificationType NotificationType => NotificationType.IssueComment;

    public CustomAnnouncement? Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        if (contract.Comment is null || contract.Issue is null || contract.Media is null)
        {
            _logger.LogError("Malformed webhook contract. Null checks - Contract.Comment={CommentIsNull},Contract.Issue={IssueIsNull},Contract.Media={MediaIsNull}", contract.Comment is null,
                contract.Issue is null, contract.Media is null);
            return null;
        }

        string username = contract.Comment.CommentedByUsername;
        string email = contract.Comment.CommentedByEmail;
        string issueType = contract.Issue.IssueType.ToString().ToLower();
        string mediaType = contract.Media.MediaType.ToString().ToLower();
        string subject = contract.Subject;
        string commentMessage = contract.Comment.CommentMessage;

        // should look like - User a_user (user@example.com) commented on audio issue for movie media item titled Deadpool (2016). Comment reads "issue with audio sync"
        var message = $"User {username} ({email}) commented on {issueType} issue for {mediaType} media item titled {subject}. Comment reads \"{commentMessage}\"";

        return new CustomAnnouncement
        {
            Title = $"New comment on issue {contract.Issue.IssueId}",
            Message = message,
            Image = contract.Image,
            Link = $"issues/{contract.Issue.IssueId}",
        };
    }
}