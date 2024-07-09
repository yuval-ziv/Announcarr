namespace Announcarr.Clients.Sonarr.Responses;

public class AddSeriesOptions
{
    public bool IgnoreEpisodesWithFiles { get; set; }
    public bool IgnoreEpisodesWithoutFiles { get; set; }
    public MonitorTypes Monitor { get; set; }
    public bool SearchForMissingEpisodes { get; set; }
    public bool SearchForCutoffUnmetEpisodes { get; set; }
}