namespace Announcarr.Clients.Radarr.Responses;

public class MovieResource
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? OriginalTitle { get; set; }
    public Language Language { get; set; } = new();
    public List<AlternativeTitleResource>? AlternateTitles { get; set; }
    public int? SecondaryYear { get; set; }
    public int SecondaryYearSourceId { get; set; }
    public string? SortTitle { get; set; }
    public long? SizeOnDisk { get; set; }
    public MovieStatusType Status { get; set; }
    public string? Overview { get; set; }
    public DateTimeOffset? InCinemas { get; set; }
    public DateTimeOffset? PhysicalRelease { get; set; }
    public DateTimeOffset? DigitalRelease { get; set; }
    public string? PhysicalReleaseNote { get; set; }
    public List<MediaCover>? Images { get; set; }
    public string? Website { get; set; }
    public string? RemotePoster { get; set; }
    public int Year { get; set; }
    public string? YouTubeTrailerId { get; set; }
    public string? Studio { get; set; }
    public string? Path { get; set; }
    public int QualityProfileId { get; set; }
    public bool? HasFile { get; set; }
    public int MovieFileId { get; set; }
    public bool Monitored { get; set; }
    public MovieStatusType MinimumAvailability { get; set; }
    public bool IsAvailable { get; set; }
    public string? FolderName { get; set; }
    public int Runtime { get; set; }
    public string? CleanTitle { get; set; }
    public string? ImdbId { get; set; }
    public int TmdbId { get; set; }
    public string? TitleSlug { get; set; }
    public string? RootFolderPath { get; set; }
    public string? Folder { get; set; }
    public string? Certification { get; set; }
    public List<string>? Genres { get; set; }
    public HashSet<int>? Tags { get; set; }
    public DateTimeOffset? Added { get; set; }
    public AddMovieOptions AddOptions { get; set; } = new();
    public Ratings Ratings { get; set; } = new();
    public MovieFileResource MovieFile { get; set; } = new();
    public MovieCollectionResource Collection { get; set; } = new();
    public float Popularity { get; set; }
    public MovieStatisticsResource Statistics { get; set; } = new();
}