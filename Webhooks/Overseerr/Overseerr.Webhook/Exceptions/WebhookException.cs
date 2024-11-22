namespace Announcarr.Webhooks.Overseerr.Webhook.Exceptions;

public class WebhookException : Exception
{
    public WebhookException()
    {
    }

    public WebhookException(string? message) : base(message)
    {
    }

    public WebhookException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}