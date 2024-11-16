﻿using Announcarr.Abstractions.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Announcarr.Webhooks.Overseerr.Webhook.Handlers;

public class IssueResolvedOverseerrWebhookHandler : IOverseerrWebhookHandler
{
    private readonly ILogger<IssueResolvedOverseerrWebhookHandler> _logger;

    public IssueResolvedOverseerrWebhookHandler(ILogger<IssueResolvedOverseerrWebhookHandler> logger)
    {
        _logger = logger;
    }

    public NotificationType NotificationType => NotificationType.IssueResolved;

    public CustomAnnouncement? Handle(OverseerrWebhookContract contract, CancellationToken cancellationToken = default)
    {
        if (contract.Issue is null || contract.Media is null)
        {
            _logger.LogError("Malformed webhook contract. Null checks - Contract.Issue={IssueIsNull},Contract.Media={MediaIsNull}", contract.Issue is null, contract.Media is null);
            return null;
        }

        string issueType = contract.Issue.IssueType.ToString().ToLower();
        string mediaType = contract.Media.MediaType.ToString().ToLower();
        string subject = contract.Subject;

        // should look like - Issue resolved of type audio for movie media item titled Deadpool (2016).
        var message = $"Issue resolved of type {issueType} for {mediaType} media item titled {subject}.";

        return new CustomAnnouncement
        {
            Title = "Issue resolved",
            Message = message,
            Image = contract.Image,
            Link = $"issues/{contract.Issue.IssueId}",
        };
    }
}