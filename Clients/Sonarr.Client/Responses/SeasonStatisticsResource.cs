namespace Announcarr.Clients.Sonarr.Responses;

public class SeasonStatisticsResource
{
    public DateTimeOffset? NextAiring { get; set; }
    public DateTimeOffset? PreviousAiring { get; set; }
    public int EpisodeFileCount { get; set; }
    public int EpisodeCount { get; set; }
    public int TotalEpisodeCount { get; set; }
    public int SizeOnDisk { get; set; }
    public List<string>? ReleaseGroups { get; set; }
    public double PercentOfEpisodes { get; set; }
}