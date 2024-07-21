namespace Announcarr.Clients.Radarr.Responses;

public class MovieStatisticsResource
{
    public int MovieFileCount { get; set; }
    public long SizeOnDisk { get; set; }
    public List<string>? ReleaseGroups { get; set; }
}