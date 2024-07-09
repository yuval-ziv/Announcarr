namespace Announcarr.Clients.Radarr.Responses;

public class MovieFileResource
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public string? RelativePath { get; set; }
    public string? Path { get; set; }
    public long Size { get; set; }
    public DateTimeOffset DateAdded { get; set; }
    public string? SceneName { get; set; }
    public string? ReleaseGroup { get; set; }
    public string? Edition { get; set; }
    public List<Language>? Languages { get; set; }
    public QualityModel QualityModel { get; set; } = new();
    public List<CustomFormatResource>? CustomFormats { get; set; }
    public int CustomFormatScore { get; set; }
    public int? IndexerFlags { get; set; }
    public MediaInfoResource MediaInfo { get; set; } = new();
    public string? OriginalFilePath { get; set; }
    public bool QualityCutoffNotMet { get; set; }
}