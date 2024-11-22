using Announcarr.Webhooks.Overseerr.Webhook.Contracts.Enums;

namespace Announcarr.Webhooks.Overseerr.Webhook.Contracts;

public class Media
{
    public MediaType MediaType { get; set; }
    public string? TmdbId { get; set; }
    public string? TvdbId { get; set; }
    public MediaStatus Status { get; set; }
    public MediaStatus Status4K { get; set; }
}