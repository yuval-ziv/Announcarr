namespace Announcer.Clients.Radarr.Responses;

public class AlternativeTitleResource
{
    public int Id { get; set; }
    public SourceType SourceType { get; set; }
    public int MovieMetadataId { get; set; }
    public string? Title { get; set; }
    public string? CleanTitle { get; set; }
}