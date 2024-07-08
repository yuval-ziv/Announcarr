namespace Announcer.Clients.Sonarr.Responses;

public class SeasonResource
{
    public int SeasonNumber { get; set; }
    public bool Monitored { get; set; }
    public SeasonStatisticsResource Statistics { get; set; } = new();
    public List<MediaCover>? Images { get; set; }
}