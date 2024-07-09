namespace Announcarr.Clients.Sonarr.Responses;

public class EpisodeResource
{
    public int Id { get; set; }
    public int SeriesId { get; set; }
    public int TvdbId { get; set; }
    public int EpisodeFileId { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string? Title { get; set; }
    public string? AirDate { get; set; }
    public DateTimeOffset? AirDateUtc { get; set; }
    public int Runtime { get; set; }
    public string? FinaleType { get; set; }
    public string? Overview { get; set; }
    public EpisodeFileResource? EpisodeFile { get; set; }
    public bool HasFile { get; set; }
    public bool Monitored { get; set; }
    public int AbsoluteEpisodeNumber { get; set; }
    public int SceneAbsoluteEpisodeNumber { get; set; }
    public int SceneEpisodeNumber { get; set; }
    public int SceneSeasonNumber { get; set; }
    public bool UnverifiedSceneNumbering { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public DateTimeOffset? GrabDate { get; set; }
    public SeriesResource? Series { get; set; }
    public List<MediaCover>? Images { get; set; }
}