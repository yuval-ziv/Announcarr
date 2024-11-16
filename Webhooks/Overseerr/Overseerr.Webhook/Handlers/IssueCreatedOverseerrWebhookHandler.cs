﻿using Announcarr.Abstractions.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class IssueCreatedOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    private readonly ILogger<IssueCreatedOverseerrWebhookHandler> _logger;

    public IssueCreatedOverseerrWebhookHandler(ILogger<IssueCreatedOverseerrWebhookHandler> logger)
    {
        _logger = logger;
    }

    public NotificationType NotificationType => NotificationType.IssueCreated;

    public CustomAnnouncement? Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        if (contract.Issue is null || contract.Media is null)
        {
            _logger.LogError("Malformed webhook contract. Null checks - Contract.Issue={IssueIsNull},Contract.Media={MediaIsNull}", contract.Issue is null, contract.Media is null);
            return null;
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