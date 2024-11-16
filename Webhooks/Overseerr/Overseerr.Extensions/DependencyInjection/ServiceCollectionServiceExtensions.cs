using System.Diagnostics.CodeAnalysis;
using Announcarr.Webhooks.Overseerr.Webhook.Handlers;
using Announcarr.Webhooks.Overseerr.Webhook.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Announcarr.Webhooks.Overseerr.Extensions.DependencyInjection;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddDefaultOverseerrWebhookHandlers(this IServiceCollection services)
    {
        return services.AddSingleton<IWebhookService, WebhookService>()
            .AddOverseerrWebhookHandler<TestNotificationOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<IssueCommentOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<IssueCreatedOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<IssueReopenedOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<IssueResolvedOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<MediaApprovedOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<MediaAutoApprovedOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<MediaAutoRequestedOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<MediaAvailableOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<MediaDeclinedOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<MediaFailedOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<MediaPendingOverseerrWebhookHandler>()
            .AddOverseerrWebhookHandler<TestNotificationOverseerrWebhookHandler>();
    }

    private static IServiceCollection AddOverseerrWebhookHandler<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
        where TImplementation : class, IOverseerrWebhookHandler
    {
        return services.AddSingleton<IOverseerrWebhookHandler, TImplementation>();
    }
}