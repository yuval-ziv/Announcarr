namespace Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;

public enum NotificationType
{
    None,
    MediaPending,
    MediaApproved,
    MediaAvailable,
    MediaFailed,
    TestNotification,
    MediaDeclined,
    MediaAutoApproved,
    IssueCreated,
    IssueComment,
    IssueResolved,
    IssueReopened,
    MediaAutoRequested,
}