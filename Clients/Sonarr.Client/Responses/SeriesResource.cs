namespace Announcarr.Clients.Sonarr.Responses;

public class SeriesResource
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public List<AlternateTitleResource>? AlternateTitles { get; set; }
    public string? SortTitle { get; set; }
    public SeriesStatusType Status { get; set; }
    public bool Ended { get; set; }
    public string? ProfileName { get; set; }
    public string? Overview { get; set; }
    public DateTimeOffset? NextAiring { get; set; }
    public DateTimeOffset? PreviousAiring { get; set; }
    public string? Network { get; set; }
    public string? AirTime { get; set; }
    public List<MediaCover>? Images { get; set; }
    public Language OriginalLanguage { get; set; } = new();
    public string? RemotePoster { get; set; }
    public List<SeasonResource>? Seasons { get; set; }
    public int Year { get; set; }
    public string? Path { get; set; }
    public int QualityProfileId { get; set; }
    public bool SeasonFolder { get; set; }
    public bool Monitored { get; set; }
    public NewItemMonitorTypes MonitorNewItems { get; set; }
    public bool UseSceneNumbering { get; set; }
    public int Runtime { get; set; }
    public int TvdbId { get; set; }
    public int TvRageId { get; set; }
    public int TvMazeId { get; set; }
    public DateTimeOffset? FirstAired { get; set; }
    public DateTimeOffset? LastAired { get; set; }
    public SeriesTypes SeriesType { get; set; }
    public string? CleanTitle { get; set; }
    public string? ImdbId { get; set; }
    public string? TitleSlug { get; set; }
    public string? RootFolderPath { get; set; }
    public string? Folder { get; set; }
    public string? Certification { get; set; }
    public List<string> Genres { get; set; } = [];
    public HashSet<int> Tags { get; set; } = [];
    public DateTimeOffset? Added { get; set; }
    public AddSeriesOptions AddOptions { get; set; } = new();
    public Ratings Ratings { get; set; } = new();
    public SeriesStatisticsResource Statistics { get; set; } = new();
    public bool? EpisodesChanged { get; set; }

    [Obsolete("This property is marked as obsolete in the Sonarr.Integration V3 API docs.")]
    public int LanguageProfileId { get; set; }
}