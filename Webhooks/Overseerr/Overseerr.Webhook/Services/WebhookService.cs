using Announcarr.Abstractions.Contracts;
using Announcarr.Exporters.Abstractions.Exporter.Interfaces;
using Announcarr.Webhooks.Overseerr.Webhook.Contracts;
using Announcarr.Webhooks.Overseerr.Webhook.Handlers;
using Microsoft.Extensions.Logging;

namespace Announcarr.Webhooks.Overseerr.Webhook.Services;

public class WebhookService : IWebhookService
{
    private const string OverseerrDefaultUrl = "http://localhost:5055";

    private readonly List<IExporterService> _exporters;
    private readonly List<IOverseerrWebhookHandler> _handlers;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(ILogger<WebhookService> logger, IEnumerable<IOverseerrWebhookHandler> handlers, IEnumerable<IExporterService> exporters)
    {
        _logger = logger;
        _exporters = exporters.ToList();
        _handlers = handlers.ToList();
    }

    public Task HandleAsync(OverseerrWebhookContract contract, string overseerrUrl = OverseerrDefaultUrl, string? webhookName = "Overseerr", bool isEnabled = true,
        CancellationToken cancellationToken = default)
    {
        return HandleAsync(contract, [], overseerrUrl, webhookName, isEnabled, cancellationToken);
    }

    public async Task<bool> HandleAsync(OverseerrWebhookContract contract, HashSet<string> tags, string overseerrUrl = OverseerrDefaultUrl, string? webhookName = "Overseerr", bool isEnabled = true,
        CancellationToken cancellationToken = default)
    {
        if (!isEnabled)
        {
            _logger.LogDebug("[{WebhookName}] Got webhook request of type {NotificationType} but handler is disabled", webhookName, contract.NotificationType);
            return true;
        }

        IOverseerrWebhookHandler? selectedHandler = _handlers.FirstOrDefault(handler => handler.NotificationType == contract.NotificationType);

        if (selectedHandler is null)
        {
            _logger.LogError("[{WebhookName}] No handler found for notification type {NotificationType}", webhookName, contract.NotificationType);
            return false;
        }

        CustomAnnouncement? message = selectedHandler.Handle(contract, cancellationToken);

        if (message is null)
        {
            return false;
        }

        message.Tags = tags;
        message.Link = message.Link is not null ? MergeUrlWithPath(overseerrUrl, message.Link) : null;

        await Task.WhenAll(_exporters.Select(exporter => exporter.ExportCustomAnnouncementAsync(message, cancellationToken)));
        return true;
    }

    private static string MergeUrlWithPath(string overseerrUrl, string? path)
    {
        return new Uri(new Uri(overseerrUrl), path).ToString();
    }
}