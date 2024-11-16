namespace Announcarr.Webhooks.Overseerr.Webhook.Contracts;

public class IssueComment
{
    public required string CommentedByUsername { get; set; }
    public required string CommentedByEmail { get; set; }
    public required string CommentedByAvatar { get; set; }
    public required string CommentMessage { get; set; }
}