namespace Announcarr.Webhooks.Overseerr.Webhook.Exceptions;

public class WebhookMalformedContractException : WebhookException
{
    public WebhookMalformedContractException(string? message) : base(message)
    {
    }
}