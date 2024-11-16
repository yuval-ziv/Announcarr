using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;

namespace Announcarr.Webhooks.Overseerr.Webhook.Contracts;

public class Issue
{
    public required int IssueId { get; set; }
    public required IssueType IssueType { get; set; }
    public IssueStatus IssueStatus { get; set; } = IssueStatus.Open;
    public required string ReportedByUsername { get; set; }
    public required string ReportedByEmail { get; set; }
    public required string ReportedByAvatar { get; set; }
}