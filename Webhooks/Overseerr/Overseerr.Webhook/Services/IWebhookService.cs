using Announcarr.Webhooks.Overseerr.Webhook.Contracts;

namespace Announcarr.Webhooks.Overseerr.Webhook.Services;

public interface IWebhookService
{
    private const string OverseerrDefaultUrl = "http://localhost:5055";
    
    Task HandleAsync(OverseerrWebhookContract contract, string overseerrUrl = OverseerrDefaultUrl, string? webhookName = "Overseerr", bool isEnabled = true,
        CancellationToken cancellationToken = default);

    Task<bool> HandleAsync(OverseerrWebhookContract contract, HashSet<string> tags, string overseerrUrl = OverseerrDefaultUrl, string? webhookName = "Overseerr", bool isEnabled = true,
        CancellationToken cancellationToken = default);
}