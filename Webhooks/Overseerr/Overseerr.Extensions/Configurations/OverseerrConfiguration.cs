namespace Announcarr.Webhooks.Overseerr.Extensions.Configurations;

public class OverseerrConfiguration
{
    public const string SectionName = "Overseerr";

    public bool EnableWebhookListener { get; set; } = true;
    public bool AllowRemoteRequests { get; set; } = true;
    public string Method { get; set; } = HttpMethod.Post.Method;
    public string Path { get; set; } = "/overseerr/webhook";
    public string OverseerrHost { get; set; } = "http://localhost:5055";
    public OverseerrWebhookConfiguration Webhook { get; set; } = new();
}