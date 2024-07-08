namespace Announcer.Clients.Sonarr.Responses;

public class EpisodeFileResource
{
    public int Id { get; set; }
    public int SeriesId { get; set; }
    public int SeasonNumber { get; set; }
    public string? RelativePath { get; set; }
    public string? Path { get; set; }
    public long Size { get; set; }
    public DateTimeOffset? DateAdded { get; set; }
    public string? SceneName { get; set; }
    public string? ReleaseGroup { get; set; }
    public List<Language>? Languages { get; set; }
    public QualityModel Quality { get; set; } = new();
    public List<CustomFormatResource>? CustomFormats { get; set; }
    public int CustomFormatScore { get; set; }
    public int IndexerFlags { get; set; }
    public ReleaseType ReleaseType { get; set; }
    public MediaInfoResource MediaInfo { get; set; } = new();
    public bool QualityCutoffNotMet { get; set; }
}